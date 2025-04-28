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
    /// <para type="synopsis">Gets users from Proxmox VE.</para>
    /// <para type="description">The Get-ProxmoxUser cmdlet retrieves users from Proxmox VE.</para>
    /// <example>
    ///   <para>Get all users</para>
    ///   <code>$users = Get-ProxmoxUser -Connection $connection</code>
    /// </example>
    /// <example>
    ///   <para>Get a specific user by ID</para>
    ///   <code>$user = Get-ProxmoxUser -Connection $connection -UserID "root@pam"</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "ProxmoxUser")]
    [OutputType(typeof(ProxmoxUser), typeof(string))]
    public class GetProxmoxUserCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The ID of the user to retrieve.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string UserID { get; set; }

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

                if (string.IsNullOrEmpty(UserID))
                {
                    // Get all users
                    response = client.Get("access/users");
                    var usersData = JsonUtility.DeserializeResponse<JArray>(response);
                    var users = new List<ProxmoxUser>();

                    foreach (var userObj in usersData)
                    {
                        var user = userObj.ToObject<ProxmoxUser>();
                        users.Add(user);
                    }

                    if (RawJson.IsPresent)
                    {
                        WriteObject(response);
                    }
                    else
                    {
                        WriteObject(users, true);
                    }
                }
                else
                {
                    // Get a specific user
                    response = client.Get($"access/users/{Uri.EscapeDataString(UserID)}");
                    var userData = JsonUtility.DeserializeResponse<JObject>(response);
                    var user = userData.ToObject<ProxmoxUser>();

                    if (RawJson.IsPresent)
                    {
                        WriteObject(response);
                    }
                    else
                    {
                        WriteObject(user);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetProxmoxUserError", ErrorCategory.OperationStopped, Connection));
            }
        }
    }
}
