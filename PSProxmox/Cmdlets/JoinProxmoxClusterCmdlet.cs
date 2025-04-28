using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Security;
using PSProxmox.Client;
using PSProxmox.Session;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Joins a node to a Proxmox VE cluster.</para>
    /// <para type="description">The Join-ProxmoxCluster cmdlet joins a node to a Proxmox VE cluster.</para>
    /// <example>
    ///   <para>Join a node to a cluster</para>
    ///   <code>Join-ProxmoxCluster -Connection $connection -ClusterName "cluster1" -HostName "pve2" -Password $securePassword</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Join, "ProxmoxCluster", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class JoinProxmoxClusterCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The name of the cluster to join.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public string ClusterName { get; set; }

        /// <summary>
        /// <para type="description">The hostname or IP address of an existing cluster member.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public string HostName { get; set; }

        /// <summary>
        /// <para type="description">The password for the root@pam user on the existing cluster member.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public SecureString Password { get; set; }

        /// <summary>
        /// <para type="description">Whether to force joining the cluster.</para>
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

                // Confirm joining the cluster
                if (!ShouldProcess($"Node {Connection.Server}", $"Join cluster {ClusterName}"))
                {
                    return;
                }

                // Convert SecureString to plain text (only for sending to API)
                string plainPassword = new System.Net.NetworkCredential(string.Empty, Password).Password;

                // Join the cluster
                var parameters = new Dictionary<string, string>
                {
                    ["hostname"] = HostName,
                    ["password"] = plainPassword
                };

                if (Force.IsPresent)
                {
                    parameters["force"] = "1";
                }

                WriteVerbose($"Joining cluster {ClusterName} via host {HostName}");
                client.Post("cluster/join", parameters);

                WriteVerbose($"Node {Connection.Server} joined cluster {ClusterName}");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "JoinProxmoxClusterError", ErrorCategory.OperationStopped, Connection));
            }
        }
    }
}
