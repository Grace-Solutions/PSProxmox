using Newtonsoft.Json;
using System.Collections.Generic;

namespace PSProxmox.Models
{
    /// <summary>
    /// Represents a user in Proxmox VE.
    /// </summary>
    public class ProxmoxUser
    {
        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        [JsonProperty("userid")]
        public string UserID { get; set; }

        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        [JsonProperty("firstname")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        [JsonProperty("lastname")]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        [JsonProperty("email")]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is enabled.
        /// </summary>
        [JsonProperty("enable")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is an administrator.
        /// </summary>
        [JsonProperty("isadmin")]
        public bool IsAdmin { get; set; }

        /// <summary>
        /// Gets or sets the user's comment.
        /// </summary>
        [JsonProperty("comment")]
        public string Comment { get; set; }

        /// <summary>
        /// Gets or sets the user's expiration date.
        /// </summary>
        [JsonProperty("expire")]
        public long Expire { get; set; }

        /// <summary>
        /// Gets or sets the user's groups.
        /// </summary>
        [JsonProperty("groups")]
        public List<string> Groups { get; set; }

        /// <summary>
        /// Gets or sets the user's realm.
        /// </summary>
        [JsonProperty("realm")]
        public string Realm { get; set; }

        /// <summary>
        /// Gets the user's full name.
        /// </summary>
        [JsonIgnore]
        public string FullName => $"{FirstName} {LastName}".Trim();

        /// <summary>
        /// Gets the user's username.
        /// </summary>
        [JsonIgnore]
        public string Username => UserID?.Split('@')[0];

        /// <summary>
        /// Gets a value indicating whether the user has expired.
        /// </summary>
        [JsonIgnore]
        public bool HasExpired => Expire > 0 && Expire < System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    }
}
