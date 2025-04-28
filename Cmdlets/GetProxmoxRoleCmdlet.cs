using System;
using System.Collections.Generic;
using System.Management.Automation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PSProxmox.Client;
using PSProxmox.Models;
using PSProxmox.Session;
using PSProxmox.Utilities;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Gets roles from Proxmox VE.</para>
    /// <para type="description">The Get-ProxmoxRole cmdlet retrieves roles from Proxmox VE.</para>
    /// <example>
    ///   <para>Get all roles</para>
    ///   <code>$roles = Get-ProxmoxRole -Connection $connection</code>
    /// </example>
    /// <example>
    ///   <para>Get a specific role by ID</para>
    ///   <code>$role = Get-ProxmoxRole -Connection $connection -RoleID "Administrator"</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "ProxmoxRole")]
    [OutputType(typeof(ProxmoxRole), typeof(string))]
    public class GetProxmoxRoleCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The ID of the role to retrieve.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string RoleID { get; set; }

        /// <summary>
        /// <para type="description">Whether to return the raw JSON response.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter RawJson { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);
                string response;

                if (string.IsNullOrEmpty(RoleID))
                {
                    // Get all roles
                    response = client.Get("access/roles");
                    var rolesData = JsonUtility.DeserializeResponse<JArray>(response);
                    var roles = new List<ProxmoxRole>();

                    foreach (var roleObj in rolesData)
                    {
                        var role = roleObj.ToObject<ProxmoxRole>();
                        roles.Add(role);
                    }

                    if (RawJson.IsPresent)
                    {
                        WriteObject(response);
                    }
                    else
                    {
                        WriteObject(roles, true);
                    }
                }
                else
                {
                    // Get a specific role
                    response = client.Get($"access/roles/{Uri.EscapeDataString(RoleID)}");
                    var roleData = JsonUtility.DeserializeResponse<JObject>(response);
                    var role = roleData.ToObject<ProxmoxRole>();

                    if (RawJson.IsPresent)
                    {
                        WriteObject(response);
                    }
                    else
                    {
                        WriteObject(role);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetProxmoxRoleError", ErrorCategory.OperationStopped, Connection));
            }
        }
    }
}
