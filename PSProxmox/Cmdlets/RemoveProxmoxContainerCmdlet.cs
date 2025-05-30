using System;
using System.Collections.Generic;
using System.Management.Automation;
using Newtonsoft.Json.Linq;
using PSProxmox.Client;
using PSProxmox.Session;
using PSProxmox.Utilities;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Removes a Proxmox LXC container</para>
    /// <para type="description">Removes a Proxmox LXC container from a Proxmox server</para>
    /// </summary>
    /// <example>
    /// <code>Remove-ProxmoxContainer -Node "pve1" -CTID 100</code>
    /// <para>Removes the container with CTID 100 from node pve1</para>
    /// </example>
    /// <example>
    /// <code>Remove-ProxmoxContainer -Node "pve1" -CTID 100 -Confirm:$false</code>
    /// <para>Removes the container with CTID 100 from node pve1 without confirmation</para>
    /// </example>
    [Cmdlet(VerbsCommon.Remove, "ProxmoxContainer", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveProxmoxContainerCmdlet : ProxmoxCmdlet
    {
        /// <summary>
        /// Gets or sets the node name
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        public string Node { get; set; }

        /// <summary>
        /// Gets or sets the container ID
        /// </summary>
        [Parameter(Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true)]
        public int CTID { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to force removal
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
                if (ShouldProcess($"Container {CTID} on node {Node}", "Remove"))
                {
                    var client = GetProxmoxClient();
                    var endpoint = $"nodes/{Node}/lxc/{CTID}";

                    if (Force.IsPresent)
                    {
                        endpoint += "?force=1";
                    }

                    var response = client.Delete(endpoint);
                    var responseData = JsonUtility.DeserializeResponse<JObject>(response);
                    var taskId = responseData["upid"]?.ToString();

                    var taskStatus = WaitForTask(Node, taskId);
                    if (taskStatus != "OK")
                    {
                        throw new Exception($"Failed to remove container: {taskStatus}");
                    }

                    WriteVerbose($"Container {CTID} on node {Node} removed successfully");
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "RemoveProxmoxContainerError", ErrorCategory.OperationStopped, null));
            }
        }

        private string WaitForTask(string node, string taskId)
        {
            var client = GetProxmoxClient();
            var status = "";
            var attempts = 0;
            var maxAttempts = 60; // Wait up to 60 seconds

            while (attempts < maxAttempts)
            {
                var response = client.Get($"nodes/{node}/tasks/{taskId}/status");
                var data = JsonUtility.DeserializeResponse<JObject>(response);
                status = data["status"]?.ToString();

                if (status == "stopped")
                {
                    return data["exitstatus"]?.ToString() ?? "OK";
                }

                System.Threading.Thread.Sleep(1000);
                attempts++;
            }

            throw new Exception("Timeout waiting for task to complete");
        }
    }
}
