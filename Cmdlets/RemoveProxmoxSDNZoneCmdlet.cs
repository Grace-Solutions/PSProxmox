using System;
using System.Management.Automation;
using PSProxmox.Client;
using PSProxmox.Session;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Removes an SDN zone from Proxmox VE.</para>
    /// <para type="description">The Remove-ProxmoxSDNZone cmdlet removes an SDN zone from Proxmox VE.</para>
    /// <example>
    ///   <para>Remove an SDN zone</para>
    ///   <code>Remove-ProxmoxSDNZone -Connection $connection -Zone "zone1"</code>
    /// </example>
    /// <example>
    ///   <para>Remove an SDN zone using pipeline input</para>
    ///   <code>Get-ProxmoxSDNZone -Connection $connection -Zone "zone1" | Remove-ProxmoxSDNZone -Connection $connection</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "ProxmoxSDNZone", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveProxmoxSDNZoneCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The name of the zone to remove.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Zone { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);

                // Confirm removal
                if (!ShouldProcess($"SDN zone {Zone}", "Remove"))
                {
                    return;
                }

                // Remove the zone
                WriteVerbose($"Removing SDN zone {Zone}");
                client.Delete($"sdn/zones/{Zone}");

                WriteVerbose($"SDN zone {Zone} removed");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "RemoveProxmoxSDNZoneError", ErrorCategory.OperationStopped, Zone));
            }
        }
    }
}
