using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using PSProxmox.Client;
using PSProxmox.Models;
using PSProxmox.Session;
using PSProxmox.Utilities;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Gets TurnKey Linux templates for Proxmox LXC containers</para>
    /// <para type="description">Gets TurnKey Linux templates for Proxmox LXC containers from a Proxmox server</para>
    /// <para type="description">If no parameters are specified, all templates are returned</para>
    /// <para type="description">If a node is specified, only templates available on that node are returned</para>
    /// <para type="description">If a name is specified, only templates with that name are returned (supports wildcards and regex)</para>
    /// </summary>
    /// <example>
    /// <code>Get-ProxmoxTurnKeyTemplate</code>
    /// <para>Gets all TurnKey Linux templates from the Proxmox server</para>
    /// </example>
    /// <example>
    /// <code>Get-ProxmoxTurnKeyTemplate -Node "pve1"</code>
    /// <para>Gets all TurnKey Linux templates available on node pve1</para>
    /// </example>
    /// <example>
    /// <code>Get-ProxmoxTurnKeyTemplate -Name "wordpress*"</code>
    /// <para>Gets all TurnKey Linux templates with names starting with "wordpress" (wildcard)</para>
    /// </example>
    /// <example>
    /// <code>Get-ProxmoxTurnKeyTemplate -Name "^wordpress\d+$"</code>
    /// <para>Gets all TurnKey Linux templates with names matching the regex pattern "^wordpress\d+$"</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "ProxmoxTurnKeyTemplate")]
    [OutputType(typeof(ProxmoxTurnKeyTemplate))]
    public class GetProxmoxTurnKeyTemplateCmdlet : ProxmoxCmdlet
    {
        /// <summary>
        /// Gets or sets the node name
        /// </summary>
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = true)]
        public string Node { get; set; }

        /// <summary>
        /// Gets or sets the template name (supports wildcards and regex)
        /// </summary>
        [Parameter(Position = 1, ValueFromPipelineByPropertyName = true)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include downloaded templates
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter IncludeDownloaded { get; set; }

        /// <summary>
        /// Processes the cmdlet
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            try
            {
                if (string.IsNullOrEmpty(Node))
                {
                    // Get all nodes
                    var nodes = GetNodes();
                    var allTemplates = new List<ProxmoxTurnKeyTemplate>();

                    foreach (var node in nodes)
                    {
                        var templates = GetTemplates(node.Name);
                        allTemplates.AddRange(templates);
                    }

                    // Remove duplicates
                    allTemplates = allTemplates.GroupBy(t => t.Name).Select(g => g.First()).ToList();

                    // Filter by name if specified
                    if (!string.IsNullOrEmpty(Name))
                    {
                        allTemplates = FilterTemplatesByName(allTemplates, Name);
                    }

                    WriteObject(allTemplates, true);
                }
                else
                {
                    // Get templates for the specified node
                    var templates = GetTemplates(Node);

                    // Filter by name if specified
                    if (!string.IsNullOrEmpty(Name))
                    {
                        templates = FilterTemplatesByName(templates, Name);
                    }

                    WriteObject(templates, true);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetProxmoxTurnKeyTemplateError", ErrorCategory.OperationStopped, null));
            }
        }

        private List<ProxmoxNode> GetNodes()
        {
            var client = GetProxmoxClient();
            var response = client.Get("nodes");
            var data = JsonUtility.DeserializeResponse<JArray>(response);

            var nodes = new List<ProxmoxNode>();
            foreach (var item in data)
            {
                var node = item.ToObject<ProxmoxNode>();
                nodes.Add(node);
            }

            return nodes;
        }

        private List<ProxmoxTurnKeyTemplate> GetTemplates(string node)
        {
            var templates = new List<ProxmoxTurnKeyTemplate>();

            try
            {
                var client = GetProxmoxClient();

                // Get available templates from the appliance info
                var response = client.Get($"nodes/{node}/aplinfo");
                var data = JsonUtility.DeserializeResponse<JArray>(response);

                foreach (var item in data)
                {
                    // Only include TurnKey templates
                    if (item["package"]?.ToString()?.Contains("turnkey") == true)
                    {
                        var template = item.ToObject<ProxmoxTurnKeyTemplate>();
                        templates.Add(template);
                    }
                }

                // Get downloaded templates if requested
                if (IncludeDownloaded.IsPresent)
                {
                    var storages = GetStorages(node);
                    foreach (var storage in storages)
                    {
                        try
                        {
                            var contentResponse = client.Get($"nodes/{node}/storage/{storage.Name}/content");
                            var contentData = JsonUtility.DeserializeResponse<JArray>(contentResponse);

                            foreach (var item in contentData)
                            {
                                if (item["content"]?.ToString() == "vztmpl" &&
                                    item["volid"]?.ToString()?.Contains("turnkey") == true)
                                {
                                    var volid = item["volid"]?.ToString();
                                    var name = System.IO.Path.GetFileNameWithoutExtension(volid.Split('/').Last());

                                    // Check if this template is already in the list
                                    if (!templates.Any(t => t.Name == name))
                                    {
                                        var template = new ProxmoxTurnKeyTemplate
                                        {
                                            Name = name,
                                            Title = name,
                                            Description = "Downloaded template",
                                            Source = storage.Name,
                                            Location = volid
                                        };

                                        templates.Add(template);
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // Skip this storage if there's an error
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Skip this node if there's an error
            }

            return templates;
        }

        private List<ProxmoxStorage> GetStorages(string node)
        {
            var client = GetProxmoxClient();
            var response = client.Get($"nodes/{node}/storage");
            var data = JsonUtility.DeserializeResponse<JArray>(response);

            var storages = new List<ProxmoxStorage>();
            foreach (var item in data)
            {
                var storage = item.ToObject<ProxmoxStorage>();
                storages.Add(storage);
            }

            return storages;
        }

        private List<ProxmoxTurnKeyTemplate> FilterTemplatesByName(List<ProxmoxTurnKeyTemplate> templates, string namePattern)
        {
            // Check if the pattern is a regex
            if (IsRegexPattern(namePattern))
            {
                var regex = new Regex(namePattern);
                return templates.Where(t => regex.IsMatch(t.Name)).ToList();
            }

            // Otherwise, treat it as a wildcard pattern
            return templates.Where(t => IsWildcardMatch(t.Name, namePattern)).ToList();
        }

        private bool IsRegexPattern(string pattern)
        {
            // Simple heuristic: if the pattern contains regex-specific characters, treat it as regex
            return pattern.StartsWith("^") || pattern.EndsWith("$") ||
                   pattern.Contains("(") || pattern.Contains(")") ||
                   pattern.Contains("[") || pattern.Contains("]") ||
                   pattern.Contains("\\");
        }

        private bool IsWildcardMatch(string input, string pattern)
        {
            // Convert wildcard pattern to regex
            string regexPattern = "^" + Regex.Escape(pattern)
                .Replace("\\*", ".*")
                .Replace("\\?", ".") + "$";

            return Regex.IsMatch(input, regexPattern, RegexOptions.IgnoreCase);
        }
    }
}
