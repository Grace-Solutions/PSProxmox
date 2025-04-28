using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Security;
using PSProxmox.Client;
using PSProxmox.Models;
using PSProxmox.Session;
using PSProxmox.Utilities;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Creates a new user in Proxmox VE.</para>
    /// <para type="description">The New-ProxmoxUser cmdlet creates a new user in Proxmox VE.</para>
    /// <example>
    ///   <para>Create a new user</para>
    ///   <code>$user = New-ProxmoxUser -Connection $connection -Username "john" -Realm "pam" -Password $securePassword -FirstName "John" -LastName "Doe" -Email "john.doe@example.com"</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "ProxmoxUser")]
    [OutputType(typeof(ProxmoxUser))]
    public class NewProxmoxUserCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The username.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public string Username { get; set; }

        /// <summary>
        /// <para type="description">The realm.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public string Realm { get; set; }

        /// <summary>
        /// <para type="description">The password.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public SecureString Password { get; set; }

        /// <summary>
        /// <para type="description">The user's first name.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string FirstName { get; set; }

        /// <summary>
        /// <para type="description">The user's last name.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string LastName { get; set; }

        /// <summary>
        /// <para type="description">The user's email address.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Email { get; set; }

        /// <summary>
        /// <para type="description">The user's comment.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Comment { get; set; }

        /// <summary>
        /// <para type="description">The user's expiration date.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public DateTime? Expire { get; set; }

        /// <summary>
        /// <para type="description">The user's groups.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string[] Groups { get; set; }

        /// <summary>
        /// <para type="description">Whether the user is enabled.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Enabled { get; set; } = true;

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);

                // Create the user
                var parameters = new Dictionary<string, string>
                {
                    ["userid"] = $"{Username}@{Realm}"
                };

                // Convert SecureString to plain text (only for sending to API)
                string plainPassword = new System.Net.NetworkCredential(string.Empty, Password).Password;
                parameters["password"] = plainPassword;

                if (!string.IsNullOrEmpty(FirstName))
                {
                    parameters["firstname"] = FirstName;
                }

                if (!string.IsNullOrEmpty(LastName))
                {
                    parameters["lastname"] = LastName;
                }

                if (!string.IsNullOrEmpty(Email))
                {
                    parameters["email"] = Email;
                }

                if (!string.IsNullOrEmpty(Comment))
                {
                    parameters["comment"] = Comment;
                }

                if (Expire.HasValue)
                {
                    parameters["expire"] = ((DateTimeOffset)Expire.Value).ToUnixTimeSeconds().ToString();
                }

                if (Groups != null && Groups.Length > 0)
                {
                    parameters["groups"] = string.Join(",", Groups);
                }

                parameters["enable"] = Enabled.IsPresent ? "1" : "0";

                // Create the user
                client.Post("access/users", parameters);

                // Get the created user
                string userResponse = client.Get($"access/users/{Uri.EscapeDataString($"{Username}@{Realm}")}");
                var user = JsonUtility.DeserializeResponse<ProxmoxUser>(userResponse);

                WriteObject(user);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "NewProxmoxUserError", ErrorCategory.OperationStopped, Connection));
            }
        }
    }
}
