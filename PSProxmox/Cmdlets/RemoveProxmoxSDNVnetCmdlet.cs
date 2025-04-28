using System;
using System.Management.Automation;
using PSProxmox.Client;
using PSProxmox.Session;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Removes an SDN VNet from Proxmox VE.</para>
    /// <para type="description">The Remove-ProxmoxSDNVnet cmdlet removes an SDN VNet from Proxmox VE.</para>
    /// <example>
    ///   <para>Remove an SDN VNet</para>
    ///   <code>Remove-ProxmoxSDNVnet -Connection $connection -VNet "vnet1"</code>
    /// </example>
    /// <example>
    ///   <para>Remove an SDN VNet using pipeline input</para>
    ///   <code>Get-ProxmoxSDNVnet -Connection $connection -VNet "vnet1" | Remove-ProxmoxSDNVnet -Connection $connection</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "ProxmoxSDNVnet", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveProxmoxSDNVnetCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The name of the VNet to remove.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string VNet { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);

                // Confirm removal
                if (!ShouldProcess($"SDN VNet {VNet}", "Remove"))
                {
                    return;
                }

                // Remove the VNet
                WriteVerbose($"Removing SDN VNet {VNet}");
                client.Delete($"sdn/vnets/{VNet}");

                WriteVerbose($"SDN VNet {VNet} removed");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "RemoveProxmoxSDNVnetError", ErrorCategory.OperationStopped, VNet));
            }
        }
    }
}
