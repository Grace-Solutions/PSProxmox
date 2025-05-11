using System;
using System.Management.Automation;
using PSProxmox.Models;
using PSProxmox.Session;

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
                var response = Connection.PostJson($"/nodes/{Node}/lxc/{CTID}/status/start", null);
                var data = response["data"];
                var taskId = (string)data["upid"];

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
            var status = "";
            var attempts = 0;
            var maxAttempts = 60; // Wait up to 60 seconds

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
