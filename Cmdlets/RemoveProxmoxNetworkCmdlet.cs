using System;
using System.Collections.Generic;
using System.Management.Automation;
using PSProxmox.Client;
using PSProxmox.Session;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Removes a network interface from Proxmox VE.</para>
    /// <para type="description">The Remove-ProxmoxNetwork cmdlet removes a network interface from Proxmox VE.</para>
    /// <example>
    ///   <para>Remove a network interface</para>
    ///   <code>Remove-ProxmoxNetwork -Connection $connection -Node "pve1" -Interface "vmbr1"</code>
    /// </example>
    /// <example>
    ///   <para>Remove a network interface using pipeline input</para>
    ///   <code>Get-ProxmoxNetwork -Connection $connection -Node "pve1" -Interface "vmbr1" | Remove-ProxmoxNetwork -Connection $connection</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "ProxmoxNetwork", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveProxmoxNetworkCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The node where the network interface is located.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Node { get; set; }

        /// <summary>
        /// <para type="description">The name of the interface to remove.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Interface { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);

                // Confirm removal
                if (!ShouldProcess($"Network interface {Interface} on node {Node}", "Remove"))
                {
                    return;
                }

                // Remove the network interface
                WriteVerbose($"Removing network interface {Interface} on node {Node}");
                client.Delete($"nodes/{Node}/network/{Interface}");

                // Apply the network configuration
                client.Put($"nodes/{Node}/network", new Dictionary<string, string>());

                WriteVerbose($"Network interface {Interface} removed from node {Node}");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "RemoveProxmoxNetworkError", ErrorCategory.OperationStopped, Interface));
            }
        }
    }
}
