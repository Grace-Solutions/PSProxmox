using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Newtonsoft.Json.Linq;
using PSProxmox.Client;
using PSProxmox.Models;
using PSProxmox.Session;
using PSProxmox.Utilities;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Downloads a TurnKey Linux template to a Proxmox server</para>
    /// <para type="description">Downloads a TurnKey Linux template to a Proxmox server for use with LXC containers</para>
    /// </summary>
    /// <example>
    /// <code>Save-ProxmoxTurnKeyTemplate -Node "pve1" -Name "wordpress" -Storage "local"</code>
    /// <para>Downloads the WordPress TurnKey Linux template to the local storage on node pve1</para>
    /// </example>
    [Cmdlet(VerbsData.Save, "ProxmoxTurnKeyTemplate")]
    [OutputType(typeof(string))]
    public class SaveProxmoxTurnKeyTemplateCmdlet : ProxmoxCmdlet
    {
        /// <summary>
        /// Gets or sets the node name
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        public string Node { get; set; }

        /// <summary>
        /// Gets or sets the template name
        /// </summary>
        [Parameter(Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the storage name
        /// </summary>
        [Parameter(Mandatory = true, Position = 2, ValueFromPipelineByPropertyName = true)]
        public string Storage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to force download even if the template already exists
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        /// <summary>
        /// Processes the cmdlet
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            try
            {
                // Get the template information
                var templates = GetTemplates(Node);
                var template = templates.FirstOrDefault(t => t.Name.Equals(Name, StringComparison.OrdinalIgnoreCase));

                if (template == null)
                {
                    throw new Exception($"Template '{Name}' not found");
                }

                // Check if the template is already downloaded
                if (!Force.IsPresent)
                {
                    var existingTemplates = GetDownloadedTemplates(Node, Storage);
                    var existingTemplate = existingTemplates.FirstOrDefault(t => t.Contains(template.Name));

                    if (existingTemplate != null)
                    {
                        WriteVerbose($"Template '{Name}' already exists at '{existingTemplate}'");
                        WriteObject(existingTemplate);
                        return;
                    }
                }

                // Download the template
                var client = GetProxmoxClient();
                var parameters = new Dictionary<string, string>
                {
                    { "template", template.URL },
                    { "storage", Storage }
                };

                var response = client.Post($"nodes/{Node}/aplinfo", parameters);
                var responseData = JsonUtility.DeserializeResponse<JObject>(response);
                var taskId = responseData["upid"]?.ToString();

                var taskStatus = WaitForTask(Node, taskId);
                if (taskStatus != "OK")
                {
                    throw new Exception($"Failed to download template: {taskStatus}");
                }

                // Get the downloaded template path
                var downloadedTemplates = GetDownloadedTemplates(Node, Storage);
                var downloadedTemplate = downloadedTemplates.FirstOrDefault(t => t.Contains(template.Name));

                if (downloadedTemplate == null)
                {
                    throw new Exception($"Failed to find downloaded template '{Name}'");
                }

                WriteVerbose($"Template '{Name}' downloaded to '{downloadedTemplate}'");
                WriteObject(downloadedTemplate);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "SaveProxmoxTurnKeyTemplateError", ErrorCategory.OperationStopped, null));
            }
        }

        private List<ProxmoxTurnKeyTemplate> GetTemplates(string node)
        {
            var templates = new List<ProxmoxTurnKeyTemplate>();

            try
            {
                // Get available templates from the appliance info
                var client = GetProxmoxClient();
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
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get templates: {ex.Message}");
            }

            return templates;
        }

        private List<string> GetDownloadedTemplates(string node, string storage)
        {
            var templates = new List<string>();

            try
            {
                var client = GetProxmoxClient();
                var response = client.Get($"nodes/{node}/storage/{storage}/content");
                var data = JsonUtility.DeserializeResponse<JArray>(response);

                foreach (var item in data)
                {
                    if (item["content"]?.ToString() == "vztmpl" &&
                        item["volid"]?.ToString()?.Contains("turnkey") == true)
                    {
                        templates.Add(item["volid"]?.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get downloaded templates: {ex.Message}");
            }

            return templates;
        }

        private string WaitForTask(string node, string taskId)
        {
            var client = GetProxmoxClient();
            var status = "";
            var attempts = 0;
            var maxAttempts = 300; // Wait up to 5 minutes

            while (attempts < maxAttempts)
            {
                var response = client.Get($"nodes/{node}/tasks/{taskId}/status");
                var data = JsonUtility.DeserializeResponse<JObject>(response);
                status = data["status"]?.ToString();

                if (status == "stopped")
                {
                    return data["exitstatus"]?.ToString() ?? "OK";
                }

                // Show progress
                if (data["pid"] != null)
                {
                    var progressResponse = client.Get($"nodes/{node}/tasks/{taskId}/log?start=0");
                    var progressData = JsonUtility.DeserializeResponse<JArray>(progressResponse);

                    foreach (var item in progressData)
                    {
                        if (item["t"]?.ToString() == "TASK_STATUS")
                        {
                            WriteProgress(new ProgressRecord(1, "Downloading template", item["t"]?.ToString()));
                        }
                    }
                }

                System.Threading.Thread.Sleep(1000);
                attempts++;
            }

            throw new Exception("Timeout waiting for task to complete");
        }
    }
}
