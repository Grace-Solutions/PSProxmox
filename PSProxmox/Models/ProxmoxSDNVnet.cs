using Newtonsoft.Json;

namespace PSProxmox.Models
{
    /// <summary>
    /// Represents an SDN VNet in Proxmox VE.
    /// </summary>
    public class ProxmoxSDNVnet
    {
        /// <summary>
        /// Gets or sets the VNet name.
        /// </summary>
        [JsonProperty("vnet")]
        public string VNet { get; set; }

        /// <summary>
        /// Gets or sets the VNet zone.
        /// </summary>
        [JsonProperty("zone")]
        public string Zone { get; set; }

        /// <summary>
        /// Gets or sets the VNet type.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the VNet alias.
        /// </summary>
        [JsonProperty("alias")]
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets the VNet VLAN ID.
        /// </summary>
        [JsonProperty("vlanid")]
        public int? VlanID { get; set; }

        /// <summary>
        /// Gets or sets the VNet tag.
        /// </summary>
        [JsonProperty("tag")]
        public int? Tag { get; set; }

        /// <summary>
        /// Gets or sets the VNet IPv4 subnet.
        /// </summary>
        [JsonProperty("ipv4")]
        public string IPv4 { get; set; }

        /// <summary>
        /// Gets or sets the VNet IPv6 subnet.
        /// </summary>
        [JsonProperty("ipv6")]
        public string IPv6 { get; set; }

        /// <summary>
        /// Gets or sets the VNet MAC address.
        /// </summary>
        [JsonProperty("mac")]
        public string MAC { get; set; }

        /// <summary>
        /// Gets or sets the VNet bridge.
        /// </summary>
        [JsonProperty("bridge")]
        public string Bridge { get; set; }

        /// <summary>
        /// Gets or sets the VNet gateway.
        /// </summary>
        [JsonProperty("gateway")]
        public string Gateway { get; set; }

        /// <summary>
        /// Gets or sets the VNet IPv6 gateway.
        /// </summary>
        [JsonProperty("gateway6")]
        public string Gateway6 { get; set; }

        /// <summary>
        /// Gets or sets the VNet MTU.
        /// </summary>
        [JsonProperty("mtu")]
        public int? MTU { get; set; }

        /// <summary>
        /// Gets or sets the VNet DHCP.
        /// </summary>
        [JsonProperty("dhcp")]
        public bool DHCP { get; set; }

        /// <summary>
        /// Gets or sets the VNet DNS.
        /// </summary>
        [JsonProperty("dns")]
        public string DNS { get; set; }

        /// <summary>
        /// Gets or sets the VNet DNS search domain.
        /// </summary>
        [JsonProperty("dnssearchdomain")]
        public string DNSSearchDomain { get; set; }

        /// <summary>
        /// Gets or sets the VNet reverse DNS.
        /// </summary>
        [JsonProperty("reversedns")]
        public string ReverseDNS { get; set; }
    }
}
