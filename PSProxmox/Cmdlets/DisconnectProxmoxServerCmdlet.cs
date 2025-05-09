using System;
using System.Management.Automation;
using PSProxmox.Session;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Disconnects from a Proxmox VE server.</para>
    /// <para type="description">The Disconnect-ProxmoxServer cmdlet terminates a connection to a Proxmox VE server.</para>
    /// <example>
    ///   <para>Disconnect from a Proxmox VE server</para>
    ///   <code>Disconnect-ProxmoxServer -Connection $connection</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommunications.Disconnect, "ProxmoxServer")]
    public class DisconnectProxmoxServerCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to disconnect from.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                if (Connection == null)
                {
                    throw new PSArgumentNullException(nameof(Connection));
                }

                ProxmoxSession.Logout(Connection, this);

                // Clear the global connection variable if it matches the disconnected connection
                var globalConnection = SessionState.PSVariable.GetValue("DefaultProxmoxConnection") as ProxmoxConnection;
                if (globalConnection != null &&
                    globalConnection.Server == Connection.Server &&
                    globalConnection.Port == Connection.Port &&
                    globalConnection.Username == Connection.Username)
                {
                    SessionState.PSVariable.Remove("DefaultProxmoxConnection");
                    WriteVerbose("Cleared default connection");
                }

                WriteVerbose($"Disconnected from {Connection.Server}");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "DisconnectProxmoxServerError", ErrorCategory.ConnectionError, Connection));
            }
        }
    }
}
