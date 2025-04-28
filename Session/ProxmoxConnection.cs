using System;
using System.Net;
using System.Security;

namespace PSProxmox.Session
{
    /// <summary>
    /// Represents a connection to a Proxmox VE server.
    /// </summary>
    public class ProxmoxConnection
    {
        /// <summary>
        /// Gets the server hostname or IP address.
        /// </summary>
        public string Server { get; private set; }

        /// <summary>
        /// Gets the port number used for the connection.
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the connection uses HTTPS.
        /// </summary>
        public bool UseSSL { get; private set; }

        /// <summary>
        /// Gets a value indicating whether to skip SSL certificate validation.
        /// </summary>
        public bool SkipCertificateValidation { get; private set; }

        /// <summary>
        /// Gets the authentication ticket for the Proxmox API.
        /// </summary>
        public string Ticket { get; internal set; }

        /// <summary>
        /// Gets the CSRF prevention token for the Proxmox API.
        /// </summary>
        public string CSRFPreventionToken { get; internal set; }

        /// <summary>
        /// Gets the username used for authentication.
        /// </summary>
        public string Username { get; private set; }

        /// <summary>
        /// Gets the realm used for authentication.
        /// </summary>
        public string Realm { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the connection is authenticated.
        /// </summary>
        public bool IsAuthenticated => !string.IsNullOrEmpty(Ticket) && !string.IsNullOrEmpty(CSRFPreventionToken);

        /// <summary>
        /// Gets the base URL for the Proxmox API.
        /// </summary>
        public string ApiUrl => $"{(UseSSL ? "https" : "http")}://{Server}:{Port}/api2/json";

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxmoxConnection"/> class.
        /// </summary>
        /// <param name="server">The server hostname or IP address.</param>
        /// <param name="port">The port number.</param>
        /// <param name="useSSL">Whether to use HTTPS.</param>
        /// <param name="skipCertificateValidation">Whether to skip SSL certificate validation.</param>
        /// <param name="username">The username for authentication.</param>
        /// <param name="realm">The realm for authentication.</param>
        public ProxmoxConnection(string server, int port = 8006, bool useSSL = true, bool skipCertificateValidation = false, string username = null, string realm = "pam")
        {
            Server = server ?? throw new ArgumentNullException(nameof(server));
            Port = port;
            UseSSL = useSSL;
            SkipCertificateValidation = skipCertificateValidation;
            Username = username;
            Realm = realm;
        }

        /// <summary>
        /// Creates a copy of the connection with updated authentication information.
        /// </summary>
        /// <param name="ticket">The authentication ticket.</param>
        /// <param name="csrfPreventionToken">The CSRF prevention token.</param>
        /// <returns>A new connection object with updated authentication information.</returns>
        internal ProxmoxConnection WithAuthentication(string ticket, string csrfPreventionToken)
        {
            var connection = new ProxmoxConnection(Server, Port, UseSSL, SkipCertificateValidation, Username, Realm)
            {
                Ticket = ticket,
                CSRFPreventionToken = csrfPreventionToken
            };
            return connection;
        }
    }
}
