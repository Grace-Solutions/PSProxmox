using System;
using System.Management.Automation;
using PSProxmox.Client;
using PSProxmox.Session;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Removes a user from Proxmox VE.</para>
    /// <para type="description">The Remove-ProxmoxUser cmdlet removes a user from Proxmox VE.</para>
    /// <example>
    ///   <para>Remove a user</para>
    ///   <code>Remove-ProxmoxUser -Connection $connection -UserID "john@pam"</code>
    /// </example>
    /// <example>
    ///   <para>Remove a user using pipeline input</para>
    ///   <code>Get-ProxmoxUser -Connection $connection -UserID "john@pam" | Remove-ProxmoxUser -Connection $connection</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "ProxmoxUser", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveProxmoxUserCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The ID of the user to remove.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string UserID { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);

                // Confirm removal
                if (!ShouldProcess($"User {UserID}", "Remove"))
                {
                    return;
                }

                // Remove the user
                WriteVerbose($"Removing user {UserID}");
                client.Delete($"access/users/{Uri.EscapeDataString(UserID)}");

                WriteVerbose($"User {UserID} removed");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "RemoveProxmoxUserError", ErrorCategory.OperationStopped, UserID));
            }
        }
    }
}
