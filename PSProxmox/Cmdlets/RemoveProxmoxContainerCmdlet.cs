using System;
using System.Management.Automation;
using PSProxmox.Session;

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
                    var parameters = new System.Collections.Generic.Dictionary<string, object>();
                    
                    if (Force.IsPresent)
                    {
                        parameters["force"] = 1;
                    }

                    var response = Connection.DeleteJson($"/nodes/{Node}/lxc/{CTID}", parameters);
                    var data = response["data"];
                    var taskId = (string)data["upid"];

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
    }
}
