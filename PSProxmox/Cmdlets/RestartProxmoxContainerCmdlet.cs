using System;
using System.Collections.Generic;
using System.Management.Automation;
using PSProxmox.Models;
using PSProxmox.Session;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Restarts a Proxmox LXC container</para>
    /// <para type="description">Restarts a Proxmox LXC container on a Proxmox server</para>
    /// </summary>
    /// <example>
    /// <code>Restart-ProxmoxContainer -Node "pve1" -CTID 100</code>
    /// <para>Restarts the container with CTID 100 on node pve1</para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Restart, "ProxmoxContainer")]
    [OutputType(typeof(ProxmoxContainer))]
    public class RestartProxmoxContainerCmdlet : ProxmoxCmdlet
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
        /// Gets or sets a value indicating whether to wait for the container to restart
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
                var parameters = new Dictionary<string, object>();
                
                if (Timeout > 0)
                {
                    parameters["timeout"] = Timeout;
                }

                var response = Connection.PostJson($"/nodes/{Node}/lxc/{CTID}/status/restart", parameters);
                var data = response["data"];
                var taskId = (string)data["upid"];

                if (Wait.IsPresent)
                {
                    var taskStatus = WaitForTask(Node, taskId);
                    if (taskStatus != "OK")
                    {
                        throw new Exception($"Failed to restart container: {taskStatus}");
                    }

                    // Get the container status
                    var container = GetContainer(Node, CTID);
                    WriteObject(container);
                }
                else
                {
                    WriteVerbose($"Container {CTID} on node {Node} restart initiated");
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "RestartProxmoxContainerError", ErrorCategory.OperationStopped, null));
            }
        }

        private string WaitForTask(string node, string taskId)
        {
            var status = "";
            var attempts = 0;
            var maxAttempts = Timeout > 0 ? Timeout : 60;

            while (attempts < maxAttempts)
            {
                var response = Connection.GetJson($"/nodes/{node}/tasks/{taskId}/status");
                var data = response["data"];
                status = (string)data["status"];

                if (status == "stopped")
                {
                    return (string)data["exitstatus"];
                }

                System.Threading.Thread.Sleep(1000);
                attempts++;
            }

            throw new Exception("Timeout waiting for task to complete");
        }

        private ProxmoxContainer GetContainer(string node, int ctid)
        {
            var response = Connection.GetJson($"/nodes/{node}/lxc/{ctid}/status/current");
            var data = response["data"];
            
            var container = data.ToObject<ProxmoxContainer>();
            container.Node = node;
            container.CTID = ctid;
            
            return container;
        }
    }
}
