using Newtonsoft.Json;

namespace PSProxmox.Models
{
    /// <summary>
    /// Represents an SDN zone in Proxmox VE.
    /// </summary>
    public class ProxmoxSDNZone
    {
        /// <summary>
        /// Gets or sets the zone name.
        /// </summary>
        [JsonProperty("zone")]
        public string Zone { get; set; }

        /// <summary>
        /// Gets or sets the zone type.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the zone DNS servers.
        /// </summary>
        [JsonProperty("dns")]
        public string DNS { get; set; }

        /// <summary>
        /// Gets or sets the zone DNS search domains.
        /// </summary>
        [JsonProperty("dnszone")]
        public string DNSZone { get; set; }

        /// <summary>
        /// Gets or sets the zone DHCP server.
        /// </summary>
        [JsonProperty("dhcp")]
        public string DHCP { get; set; }

        /// <summary>
        /// Gets or sets the zone reverse DNS.
        /// </summary>
        [JsonProperty("reversedns")]
        public string ReverseDNS { get; set; }

        /// <summary>
        /// Gets or sets the zone IPv6.
        /// </summary>
        [JsonProperty("ipv6")]
        public string IPv6 { get; set; }

        /// <summary>
        /// Gets or sets the zone MTU.
        /// </summary>
        [JsonProperty("mtu")]
        public int MTU { get; set; }

        /// <summary>
        /// Gets or sets the zone VLAN awareness.
        /// </summary>
        [JsonProperty("vlanaware")]
        public bool VLANAware { get; set; }

        /// <summary>
        /// Gets or sets the zone bridge.
        /// </summary>
        [JsonProperty("bridge")]
        public string Bridge { get; set; }

        /// <summary>
        /// Gets or sets the zone controller.
        /// </summary>
        [JsonProperty("controller")]
        public string Controller { get; set; }

        /// <summary>
        /// Gets or sets the zone gateway.
        /// </summary>
        [JsonProperty("gateway")]
        public string Gateway { get; set; }

        /// <summary>
        /// Gets or sets the zone MAC prefix.
        /// </summary>
        [JsonProperty("mac_prefix")]
        public string MACPrefix { get; set; }

        /// <summary>
        /// Gets or sets the zone VNET count.
        /// </summary>
        [JsonProperty("vnet_count")]
        public int VNetCount { get; set; }
    }
}
