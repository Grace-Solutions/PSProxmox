using System;
using System.Management.Automation;
using PSProxmox.Session;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// Base class for Proxmox cmdlets that provides common functionality.
    /// </summary>
    public abstract class ProxmoxCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server. If not specified, the current connection will be used.</para>
        /// </summary>
        [Parameter(Mandatory = false, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// Gets the connection to use for the cmdlet.
        /// </summary>
        /// <returns>The connection to use.</returns>
        protected ProxmoxConnection GetConnection()
        {
            // Use the specified connection if provided
            if (Connection != null)
            {
                return Connection;
            }

            // Otherwise, try to get the default connection
            var defaultConnection = SessionState.PSVariable.GetValue("DefaultProxmoxConnection") as ProxmoxConnection;
            if (defaultConnection == null)
            {
                throw new PSArgumentException(
                    "No connection specified and no default connection found. Use Connect-ProxmoxServer first.",
                    nameof(Connection));
            }

            return defaultConnection;
        }

        /// <summary>
        /// Validates that the connection is authenticated.
        /// </summary>
        /// <param name="connection">The connection to validate.</param>
        protected void ValidateConnection(ProxmoxConnection connection)
        {
            if (connection == null)
            {
                throw new PSArgumentNullException(nameof(connection));
            }

            if (!connection.IsAuthenticated)
            {
                throw new PSArgumentException(
                    "The connection is not authenticated. Use Connect-ProxmoxServer first.",
                    nameof(connection));
            }
        }
    }
}
