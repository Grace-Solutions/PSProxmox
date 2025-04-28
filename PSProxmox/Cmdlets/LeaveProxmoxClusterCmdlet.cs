using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using PSProxmox.Client;
using PSProxmox.Session;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Removes a node from a Proxmox VE cluster.</para>
    /// <para type="description">The Leave-ProxmoxCluster cmdlet removes a node from a Proxmox VE cluster.</para>
    /// <example>
    ///   <para>Remove a node from a cluster</para>
    ///   <code>Leave-ProxmoxCluster -Connection $connection -Force</code>
    /// </example>
    /// </summary>
    [Cmdlet("Leave", "ProxmoxCluster", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class LeaveProxmoxClusterCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">Whether to force leaving the cluster.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);

                // Confirm leaving the cluster
                if (!ShouldProcess($"Node {Connection.Server}", "Leave cluster"))
                {
                    return;
                }

                // Leave the cluster
                var parameters = new Dictionary<string, string>();

                if (Force.IsPresent)
                {
                    parameters["force"] = "1";
                }

                WriteVerbose($"Removing node {Connection.Server} from cluster");
                string queryString = "";
                if (parameters.Count > 0)
                {
                    queryString = "?" + string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
                }
                client.Delete($"cluster/leave{queryString}");

                WriteVerbose($"Node {Connection.Server} removed from cluster");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "LeaveProxmoxClusterError", ErrorCategory.OperationStopped, Connection));
            }
        }
    }
}
