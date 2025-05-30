using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PSProxmox.Models
{
    /// <summary>
    /// Represents guest agent information for a Proxmox VM.
    /// </summary>
    public class ProxmoxVMGuestAgent
    {
        /// <summary>
        /// Gets or sets the IP addresses information.
        /// </summary>
        public ProxmoxVMGuestAgentIPAddresses IPAddresses { get; set; }

        /// <summary>
        /// Gets or sets the network interfaces information.
        /// </summary>
        public List<ProxmoxVMGuestAgentNetworkInterface> NetworkInterfaces { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the guest agent is available.
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the guest agent is enabled in VM configuration.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the guest agent is running.
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// Gets or sets the guest agent version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the last error message if guest agent communication failed.
        /// </summary>
        public string LastError { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxmoxVMGuestAgent"/> class.
        /// </summary>
        public ProxmoxVMGuestAgent()
        {
            IPAddresses = new ProxmoxVMGuestAgentIPAddresses();
            NetworkInterfaces = new List<ProxmoxVMGuestAgentNetworkInterface>();
            IsAvailable = false;
            IsEnabled = false;
            IsRunning = false;
            Version = string.Empty;
            LastError = string.Empty;
        }
    }

    /// <summary>
    /// Represents IP addresses information from the guest agent.
    /// </summary>
    public class ProxmoxVMGuestAgentIPAddresses
    {
        /// <summary>
        /// Gets or sets the IPv4 addresses.
        /// </summary>
        public List<string> IPv4 { get; set; }

        /// <summary>
        /// Gets or sets the IPv6 addresses.
        /// </summary>
        public List<string> IPv6 { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxmoxVMGuestAgentIPAddresses"/> class.
        /// </summary>
        public ProxmoxVMGuestAgentIPAddresses()
        {
            IPv4 = new List<string>();
            IPv6 = new List<string>();
        }
    }

    /// <summary>
    /// Represents a network interface from the guest agent.
    /// </summary>
    public class ProxmoxVMGuestAgentNetworkInterface
    {
        /// <summary>
        /// Gets or sets the interface name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the hardware address (MAC address).
        /// </summary>
        [JsonProperty("hardware-address")]
        public string HardwareAddress { get; set; }

        /// <summary>
        /// Gets or sets the IP addresses for this interface.
        /// </summary>
        [JsonProperty("ip-addresses")]
        public List<ProxmoxVMGuestAgentIPAddress> IPAddresses { get; set; }

        /// <summary>
        /// Gets or sets the interface statistics.
        /// </summary>
        [JsonProperty("statistics")]
        public ProxmoxVMGuestAgentNetworkStatistics Statistics { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxmoxVMGuestAgentNetworkInterface"/> class.
        /// </summary>
        public ProxmoxVMGuestAgentNetworkInterface()
        {
            IPAddresses = new List<ProxmoxVMGuestAgentIPAddress>();
        }
    }

    /// <summary>
    /// Represents an IP address from the guest agent.
    /// </summary>
    public class ProxmoxVMGuestAgentIPAddress
    {
        /// <summary>
        /// Gets or sets the IP address.
        /// </summary>
        [JsonProperty("ip-address")]
        public string Address { get; set; }

        /// <summary>
        /// Gets or sets the IP address type (ipv4 or ipv6).
        /// </summary>
        [JsonProperty("ip-address-type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the prefix length.
        /// </summary>
        [JsonProperty("prefix")]
        public int Prefix { get; set; }
    }

    /// <summary>
    /// Represents network interface statistics from the guest agent.
    /// </summary>
    public class ProxmoxVMGuestAgentNetworkStatistics
    {
        /// <summary>
        /// Gets or sets the number of bytes received.
        /// </summary>
        [JsonProperty("rx-bytes")]
        public long RxBytes { get; set; }

        /// <summary>
        /// Gets or sets the number of packets received.
        /// </summary>
        [JsonProperty("rx-packets")]
        public long RxPackets { get; set; }

        /// <summary>
        /// Gets or sets the number of bad packets received.
        /// </summary>
        [JsonProperty("rx-errs")]
        public long RxErrors { get; set; }

        /// <summary>
        /// Gets or sets the number of dropped packets received.
        /// </summary>
        [JsonProperty("rx-dropped")]
        public long RxDropped { get; set; }

        /// <summary>
        /// Gets or sets the number of bytes transmitted.
        /// </summary>
        [JsonProperty("tx-bytes")]
        public long TxBytes { get; set; }

        /// <summary>
        /// Gets or sets the number of packets transmitted.
        /// </summary>
        [JsonProperty("tx-packets")]
        public long TxPackets { get; set; }

        /// <summary>
        /// Gets or sets the number of packet transmit problems.
        /// </summary>
        [JsonProperty("tx-errs")]
        public long TxErrors { get; set; }

        /// <summary>
        /// Gets or sets the number of dropped packets transmitted.
        /// </summary>
        [JsonProperty("tx-dropped")]
        public long TxDropped { get; set; }
    }
}
