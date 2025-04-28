using System;
using System.Collections.Generic;
using System.Management.Automation;
using PSProxmox.Client;
using PSProxmox.Models;
using PSProxmox.Session;
using PSProxmox.Utilities;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Creates a new role in Proxmox VE.</para>
    /// <para type="description">The New-ProxmoxRole cmdlet creates a new role in Proxmox VE.</para>
    /// <example>
    ///   <para>Create a new role</para>
    ///   <code>$role = New-ProxmoxRole -Connection $connection -RoleID "Developer" -Privileges "VM.Allocate", "VM.Config.Disk", "VM.Config.CPU", "VM.PowerMgmt"</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "ProxmoxRole")]
    [OutputType(typeof(ProxmoxRole))]
    public class NewProxmoxRoleCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The role ID.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public string RoleID { get; set; }

        /// <summary>
        /// <para type="description">The role's privileges.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public string[] Privileges { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);

                // Create the role
                var parameters = new Dictionary<string, string>
                {
                    ["roleid"] = RoleID,
                    ["privs"] = string.Join(",", Privileges)
                };

                // Create the role
                client.Post("access/roles", parameters);

                // Get the created role
                string roleResponse = client.Get($"access/roles/{Uri.EscapeDataString(RoleID)}");
                var role = JsonUtility.DeserializeResponse<ProxmoxRole>(roleResponse);

                WriteObject(role);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "NewProxmoxRoleError", ErrorCategory.OperationStopped, Connection));
            }
        }
    }
}
