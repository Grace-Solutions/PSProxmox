using Newtonsoft.Json;

namespace PSProxmox.Models
{
    /// <summary>
    /// Represents an authentication ticket from the Proxmox VE API.
    /// </summary>
    public class ProxmoxTicket
    {
        /// <summary>
        /// Gets or sets the authentication ticket.
        /// </summary>
        [JsonProperty("ticket")]
        public string Ticket { get; set; }

        /// <summary>
        /// Gets or sets the CSRF prevention token.
        /// </summary>
        [JsonProperty("CSRFPreventionToken")]
        public string CSRFPreventionToken { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; set; }
    }
}
