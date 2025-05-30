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
    /// <para type="synopsis">Starts a Proxmox LXC container</para>
    /// <para type="description">Starts a Proxmox LXC container on a Proxmox server</para>
    /// </summary>
    /// <example>
    /// <code>Start-ProxmoxContainer -Node "pve1" -CTID 100</code>
    /// <para>Starts the container with CTID 100 on node pve1</para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Start, "ProxmoxContainer")]
    [OutputType(typeof(ProxmoxContainer))]
    public class StartProxmoxContainerCmdlet : ProxmoxCmdlet
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
        /// Gets or sets a value indicating whether to wait for the container to start
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Wait { get; set; }

        /// <summary>
        /// Processes the cmdlet
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            try
            {
                var client = GetProxmoxClient();
                var response = client.Post($"nodes/{Node}/lxc/{CTID}/status/start", new Dictionary<string, string>());
                var responseData = JsonUtility.DeserializeResponse<JObject>(response);
                var taskId = responseData["upid"]?.ToString();

                if (Wait.IsPresent)
                {
                    var taskStatus = WaitForTask(Node, taskId);
                    if (taskStatus != "OK")
                    {
                        throw new Exception($"Failed to start container: {taskStatus}");
                    }

                    // Get the container status
                    var container = GetContainer(Node, CTID);
                    WriteObject(container);
                }
                else
                {
                    WriteVerbose($"Container {CTID} on node {Node} start initiated");
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "StartProxmoxContainerError", ErrorCategory.OperationStopped, null));
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
