using System;
using System.Management.Automation;
using System.Security;
using PSProxmox.Models;
using PSProxmox.Session;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Connects to a Proxmox VE server.</para>
    /// <para type="description">The Connect-ProxmoxServer cmdlet establishes a connection to a Proxmox VE server and returns a connection object that can be used with other cmdlets.</para>
    /// <example>
    ///   <para>Connect to a Proxmox VE server using a credential object</para>
    ///   <code>$credential = Get-Credential
    /// $connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential $credential</code>
    /// </example>
    /// <example>
    ///   <para>Connect to a Proxmox VE server with explicit username and password</para>
    ///   <code>$securePassword = ConvertTo-SecureString "password" -AsPlainText -Force
    /// $connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Username "root" -Password $securePassword -Realm "pam"</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommunications.Connect, "ProxmoxServer")]
    [OutputType(typeof(ProxmoxConnection))]
    public class ConnectProxmoxServerCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The server hostname or IP address.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string Server { get; set; }

        /// <summary>
        /// <para type="description">The port number.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int Port { get; set; } = 8006;

        /// <summary>
        /// <para type="description">Whether to use HTTPS.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter UseSSL { get; set; } = true;

        /// <summary>
        /// <para type="description">Whether to skip SSL certificate validation.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter SkipCertificateValidation { get; set; }

        /// <summary>
        /// <para type="description">The credential object containing username and password.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Credential")]
        public PSCredential Credential { get; set; }

        /// <summary>
        /// <para type="description">The username for authentication.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "UsernamePassword")]
        public string Username { get; set; }

        /// <summary>
        /// <para type="description">The password for authentication.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "UsernamePassword")]
        public SecureString Password { get; set; }

        /// <summary>
        /// <para type="description">The realm for authentication.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Realm { get; set; } = "pam";

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                string username;
                SecureString password;

                if (ParameterSetName == "Credential")
                {
                    if (Credential == null)
                    {
                        throw new PSArgumentNullException(nameof(Credential));
                    }

                    username = Credential.UserName;
                    password = Credential.Password;
                }
                else
                {
                    username = Username;
                    password = Password;
                }

                var connection = ProxmoxSession.Login(
                    Server,
                    Port,
                    UseSSL.IsPresent,
                    SkipCertificateValidation.IsPresent,
                    username,
                    password,
                    Realm,
                    this);

                // Store the connection in a global variable for automatic use by other cmdlets
                SessionState.PSVariable.Set(new PSVariable("DefaultProxmoxConnection", connection, ScopedItemOptions.AllScope));

                // Return the connection object
                WriteObject(connection);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "ConnectProxmoxServerError", ErrorCategory.ConnectionError, Server));
            }
        }
    }
}
