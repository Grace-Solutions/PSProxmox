using System;
using System.Management.Automation;
using PSProxmox.Client;
using PSProxmox.Session;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Removes a storage from Proxmox VE.</para>
    /// <para type="description">The Remove-ProxmoxStorage cmdlet removes a storage from Proxmox VE.</para>
    /// <example>
    ///   <para>Remove a storage</para>
    ///   <code>Remove-ProxmoxStorage -Connection $connection -Name "backup"</code>
    /// </example>
    /// <example>
    ///   <para>Remove a storage using pipeline input</para>
    ///   <code>Get-ProxmoxStorage -Connection $connection -Name "backup" | Remove-ProxmoxStorage -Connection $connection</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "ProxmoxStorage", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveProxmoxStorageCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The name of the storage to remove.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string Name { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);

                // Confirm removal
                if (!ShouldProcess($"Storage {Name}", "Remove"))
                {
                    return;
                }

                // Remove the storage
                WriteVerbose($"Removing storage {Name}");
                client.Delete($"storage/{Name}");

                WriteVerbose($"Storage {Name} removed");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "RemoveProxmoxStorageError", ErrorCategory.OperationStopped, Name));
            }
        }
    }
}
