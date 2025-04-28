using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace PSProxmox.Models
{
    /// <summary>
    /// Represents a role in Proxmox VE.
    /// </summary>
    public class ProxmoxRole
    {
        /// <summary>
        /// Gets or sets the role ID.
        /// </summary>
        [JsonProperty("roleid")]
        public string RoleID { get; set; }

        /// <summary>
        /// Gets or sets the role's privileges.
        /// </summary>
        [JsonProperty("privs")]
        public string Privileges { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the role is a special role.
        /// </summary>
        [JsonProperty("special")]
        public bool Special { get; set; }

        /// <summary>
        /// Gets the role's privileges as a list.
        /// </summary>
        [JsonIgnore]
        public List<string> PrivilegesList => Privileges?.Split(',').ToList() ?? new List<string>();
    }
}
