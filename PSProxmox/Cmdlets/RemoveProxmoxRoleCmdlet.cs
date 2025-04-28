using System;
using System.Management.Automation;
using PSProxmox.Client;
using PSProxmox.Session;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Removes a role from Proxmox VE.</para>
    /// <para type="description">The Remove-ProxmoxRole cmdlet removes a role from Proxmox VE.</para>
    /// <example>
    ///   <para>Remove a role</para>
    ///   <code>Remove-ProxmoxRole -Connection $connection -RoleID "Developer"</code>
    /// </example>
    /// <example>
    ///   <para>Remove a role using pipeline input</para>
    ///   <code>Get-ProxmoxRole -Connection $connection -RoleID "Developer" | Remove-ProxmoxRole -Connection $connection</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "ProxmoxRole", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveProxmoxRoleCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The ID of the role to remove.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string RoleID { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);

                // Confirm removal
                if (!ShouldProcess($"Role {RoleID}", "Remove"))
                {
                    return;
                }

                // Remove the role
                WriteVerbose($"Removing role {RoleID}");
                client.Delete($"access/roles/{Uri.EscapeDataString(RoleID)}");

                WriteVerbose($"Role {RoleID} removed");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "RemoveProxmoxRoleError", ErrorCategory.OperationStopped, RoleID));
            }
        }
    }
}
