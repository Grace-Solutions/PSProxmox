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
    /// <para type="synopsis">Gets Proxmox LXC containers</para>
    /// <para type="description">Gets Proxmox LXC containers from a Proxmox server</para>
    /// <para type="description">If no parameters are specified, all containers are returned</para>
    /// <para type="description">If a CTID is specified, only that container is returned</para>
    /// <para type="description">If a node is specified, only containers on that node are returned</para>
    /// <para type="description">If a name is specified, only containers with that name are returned (supports wildcards and regex)</para>
    /// </summary>
    /// <example>
    /// <code>Get-ProxmoxContainer</code>
    /// <para>Gets all containers from the Proxmox server</para>
    /// </example>
    /// <example>
    /// <code>Get-ProxmoxContainer -CTID 100</code>
    /// <para>Gets the container with CTID 100</para>
    /// </example>
    /// <example>
    /// <code>Get-ProxmoxContainer -Node "pve1"</code>
    /// <para>Gets all containers on node pve1</para>
    /// </example>
    /// <example>
    /// <code>Get-ProxmoxContainer -Name "web*"</code>
    /// <para>Gets all containers with names starting with "web" (wildcard)</para>
    /// </example>
    /// <example>
    /// <code>Get-ProxmoxContainer -Name "^web\d+$"</code>
    /// <para>Gets all containers with names matching the regex pattern "^web\d+$"</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "ProxmoxContainer")]
    [OutputType(typeof(ProxmoxContainer))]
    public class GetProxmoxContainerCmdlet : ProxmoxCmdlet
    {
        /// <summary>
        /// Gets or sets the container ID
        /// </summary>
        [Parameter(Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
        public int? CTID { get; set; }

        /// <summary>
        /// Gets or sets the node name
        /// </summary>
        [Parameter(Position = 1, ValueFromPipelineByPropertyName = true)]
        public string Node { get; set; }

        /// <summary>
        /// Gets or sets the container name (supports wildcards and regex)
        /// </summary>
        [Parameter(Position = 2, ValueFromPipelineByPropertyName = true)]
        public string Name { get; set; }

        /// <summary>
        /// Processes the cmdlet
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            try
            {
                if (CTID.HasValue && !string.IsNullOrEmpty(Node))
                {
                    // Get a specific container on a specific node
                    var container = GetContainer(Node, CTID.Value);
                    if (container != null)
                    {
                        WriteObject(container);
                    }
                }
                else if (CTID.HasValue)
                {
                    // Get a specific container on any node
                    var nodes = GetNodes();
                    foreach (var node in nodes)
                    {
                        var container = GetContainer(node.Name, CTID.Value);
                        if (container != null)
                        {
                            WriteObject(container);
                            break;
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(Node))
                {
                    // Get all containers on a specific node
                    var containers = GetContainers(Node);

                    // Filter by name if specified
                    if (!string.IsNullOrEmpty(Name))
                    {
                        containers = FilterContainersByName(containers, Name);
                    }

                    WriteObject(containers, true);
                }
                else
                {
                    // Get all containers on all nodes
                    var nodes = GetNodes();
                    var allContainers = new List<ProxmoxContainer>();

                    foreach (var node in nodes)
                    {
                        var containers = GetContainers(node.Name);
                        allContainers.AddRange(containers);
                    }

                    // Filter by name if specified
                    if (!string.IsNullOrEmpty(Name))
                    {
                        allContainers = FilterContainersByName(allContainers, Name);
                    }

                    WriteObject(allContainers, true);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetProxmoxContainerError", ErrorCategory.OperationStopped, null));
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

        private List<ProxmoxContainer> GetContainers(string node)
        {
            var client = GetProxmoxClient();
            var response = client.Get($"nodes/{node}/lxc");
            var data = JsonUtility.DeserializeResponse<JArray>(response);

            var containers = new List<ProxmoxContainer>();
            foreach (var item in data)
            {
                var container = item.ToObject<ProxmoxContainer>();
                container.Node = node;
                containers.Add(container);
            }

            return containers;
        }

        private ProxmoxContainer GetContainer(string node, int ctid)
        {
            try
            {
                var client = GetProxmoxClient();
                var response = client.Get($"nodes/{node}/lxc/{ctid}/status/current");
                var data = JsonUtility.DeserializeResponse<ProxmoxContainer>(response);

                data.Node = node;
                data.CTID = ctid;

                // Get additional configuration
                var configResponse = client.Get($"nodes/{node}/lxc/{ctid}/config");
                var configData = JsonUtility.DeserializeResponse<Dictionary<string, object>>(configResponse);

                data.Config = configData;

                return data;
            }
            catch (Exception)
            {
                // Container not found on this node
                return null;
            }
        }

        private List<ProxmoxContainer> FilterContainersByName(List<ProxmoxContainer> containers, string namePattern)
        {
            // Check if the pattern is a regex
            if (IsRegexPattern(namePattern))
            {
                var regex = new Regex(namePattern);
                return containers.Where(c => regex.IsMatch(c.Name)).ToList();
            }

            // Otherwise, treat it as a wildcard pattern
            return containers.Where(c => IsWildcardMatch(c.Name, namePattern)).ToList();
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
