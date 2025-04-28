using Newtonsoft.Json;

namespace PSProxmox.Models
{
    /// <summary>
    /// Represents a network interface in Proxmox VE.
    /// </summary>
    public class ProxmoxNetwork
    {
        /// <summary>
        /// Gets or sets the interface name.
        /// </summary>
        [JsonProperty("iface")]
        public string Interface { get; set; }

        /// <summary>
        /// Gets or sets the interface type.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the interface method.
        /// </summary>
        [JsonProperty("method")]
        public string Method { get; set; }

        /// <summary>
        /// Gets or sets the interface IP address.
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the interface netmask.
        /// </summary>
        [JsonProperty("netmask")]
        public string Netmask { get; set; }

        /// <summary>
        /// Gets or sets the interface gateway.
        /// </summary>
        [JsonProperty("gateway")]
        public string Gateway { get; set; }

        /// <summary>
        /// Gets or sets the interface bridge ports.
        /// </summary>
        [JsonProperty("bridge_ports")]
        public string BridgePorts { get; set; }

        /// <summary>
        /// Gets or sets the interface bridge STP.
        /// </summary>
        [JsonProperty("bridge_stp")]
        public string BridgeSTP { get; set; }

        /// <summary>
        /// Gets or sets the interface bridge FD.
        /// </summary>
        [JsonProperty("bridge_fd")]
        public string BridgeFD { get; set; }

        /// <summary>
        /// Gets or sets the interface bond slaves.
        /// </summary>
        [JsonProperty("bond_slaves")]
        public string BondSlaves { get; set; }

        /// <summary>
        /// Gets or sets the interface bond mode.
        /// </summary>
        [JsonProperty("bond_mode")]
        public string BondMode { get; set; }

        /// <summary>
        /// Gets or sets the interface VLAN ID.
        /// </summary>
        [JsonProperty("vlan-id")]
        public string VlanID { get; set; }

        /// <summary>
        /// Gets or sets the interface VLAN raw device.
        /// </summary>
        [JsonProperty("vlan-raw-device")]
        public string VlanRawDevice { get; set; }

        /// <summary>
        /// Gets or sets the interface autostart flag.
        /// </summary>
        [JsonProperty("autostart")]
        public bool Autostart { get; set; }

        /// <summary>
        /// Gets or sets the interface active flag.
        /// </summary>
        [JsonProperty("active")]
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets the interface comments.
        /// </summary>
        [JsonProperty("comments")]
        public string Comments { get; set; }

        /// <summary>
        /// Gets or sets the interface node.
        /// </summary>
        [JsonProperty("node")]
        public string Node { get; set; }

        /// <summary>
        /// Gets a value indicating whether the interface is a bridge.
        /// </summary>
        [JsonIgnore]
        public bool IsBridge => Type == "bridge";

        /// <summary>
        /// Gets a value indicating whether the interface is a bond.
        /// </summary>
        [JsonIgnore]
        public bool IsBond => Type == "bond";

        /// <summary>
        /// Gets a value indicating whether the interface is a VLAN.
        /// </summary>
        [JsonIgnore]
        public bool IsVlan => Type == "vlan";

        /// <summary>
        /// Gets a value indicating whether the interface is a physical interface.
        /// </summary>
        [JsonIgnore]
        public bool IsPhysical => Type == "eth";

        /// <summary>
        /// Gets a value indicating whether the interface uses DHCP.
        /// </summary>
        [JsonIgnore]
        public bool IsDHCP => Method == "dhcp";

        /// <summary>
        /// Gets a value indicating whether the interface uses a static IP.
        /// </summary>
        [JsonIgnore]
        public bool IsStatic => Method == "static";
    }
}
