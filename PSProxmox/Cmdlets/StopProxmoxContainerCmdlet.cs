using System;
using System.Collections.Generic;
using System.Management.Automation;
using Newtonsoft.Json.Linq;
using PSProxmox.Client;
using PSProxmox.Models;
using PSProxmox.Session;
using PSProxmox.Utilities;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Stops a Proxmox LXC container</para>
    /// <para type="description">Stops a Proxmox LXC container on a Proxmox server</para>
    /// </summary>
    /// <example>
    /// <code>Stop-ProxmoxContainer -Node "pve1" -CTID 100</code>
    /// <para>Stops the container with CTID 100 on node pve1</para>
    /// </example>
    /// <example>
    /// <code>Stop-ProxmoxContainer -Node "pve1" -CTID 100 -Force</code>
    /// <para>Forces the container with CTID 100 on node pve1 to stop</para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Stop, "ProxmoxContainer")]
    [OutputType(typeof(ProxmoxContainer))]
    public class StopProxmoxContainerCmdlet : ProxmoxCmdlet
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
        /// Gets or sets a value indicating whether to force stop the container
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to wait for the container to stop
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Wait { get; set; }

        /// <summary>
        /// Gets or sets the timeout in seconds
        /// </summary>
        [Parameter(Mandatory = false)]
        public int Timeout { get; set; } = 60;

        /// <summary>
        /// Processes the cmdlet
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            try
            {
                var client = GetProxmoxClient();
                var parameters = new Dictionary<string, string>();

                if (Force.IsPresent)
                {
                    parameters["force"] = "1";
                }

                if (Timeout > 0)
                {
                    parameters["timeout"] = Timeout.ToString();
                }

                var response = client.Post($"nodes/{Node}/lxc/{CTID}/status/stop", parameters);
                var responseData = JsonUtility.DeserializeResponse<JObject>(response);
                var taskId = responseData["upid"]?.ToString();

                if (Wait.IsPresent)
                {
                    var taskStatus = WaitForTask(Node, taskId);
                    if (taskStatus != "OK")
                    {
                        throw new Exception($"Failed to stop container: {taskStatus}");
                    }

                    // Get the container status
                    var container = GetContainer(Node, CTID);
                    WriteObject(container);
                }
                else
                {
                    WriteVerbose($"Container {CTID} on node {Node} stop initiated");
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "StopProxmoxContainerError", ErrorCategory.OperationStopped, null));
            }
        }

        private string WaitForTask(string node, string taskId)
        {
            var client = GetProxmoxClient();
            var status = "";
            var attempts = 0;
            var maxAttempts = Timeout > 0 ? Timeout : 60;

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

        private ProxmoxContainer GetContainer(string node, int ctid)
        {
            var client = GetProxmoxClient();
            var response = client.Get($"nodes/{node}/lxc/{ctid}/status/current");
            var container = JsonUtility.DeserializeResponse<ProxmoxContainer>(response);

            container.Node = node;
            container.CTID = ctid;

            return container;
        }
    }
}
