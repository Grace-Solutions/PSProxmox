namespace PSProxmox.Models
{
    /// <summary>
    /// Represents connection information for a Proxmox VE server.
    /// </summary>
    public class ProxmoxConnectionInfo
    {
        /// <summary>
        /// Gets or sets the server hostname or IP address.
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// Gets or sets the port number.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the connection uses HTTPS.
        /// </summary>
        public bool UseSSL { get; set; }

        /// <summary>
        /// Gets or sets the username used for authentication.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the realm used for authentication.
        /// </summary>
        public string Realm { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the connection is authenticated.
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="ProxmoxConnectionInfo"/> class from a <see cref="Session.ProxmoxConnection"/> object.
        /// </summary>
        /// <param name="connection">The connection object.</param>
        /// <returns>A new connection information object.</returns>
        public static ProxmoxConnectionInfo FromConnection(Session.ProxmoxConnection connection)
        {
            return new ProxmoxConnectionInfo
            {
                Server = connection.Server,
                Port = connection.Port,
                UseSSL = connection.UseSSL,
                Username = connection.Username,
                Realm = connection.Realm,
                IsAuthenticated = connection.IsAuthenticated
            };
        }
    }
}
