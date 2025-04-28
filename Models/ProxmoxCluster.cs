using Newtonsoft.Json;
using System.Collections.Generic;

namespace PSProxmox.Models
{
    /// <summary>
    /// Represents a cluster in Proxmox VE.
    /// </summary>
    public class ProxmoxCluster
    {
        /// <summary>
        /// Gets or sets the cluster name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the cluster ID.
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the cluster nodes.
        /// </summary>
        [JsonProperty("nodes")]
        public List<ProxmoxClusterNode> Nodes { get; set; }

        /// <summary>
        /// Gets or sets the cluster quorum.
        /// </summary>
        [JsonProperty("quorum")]
        public ProxmoxClusterQuorum Quorum { get; set; }

        /// <summary>
        /// Gets or sets the cluster version.
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the cluster config version.
        /// </summary>
        [JsonProperty("config_version")]
        public int ConfigVersion { get; set; }
    }

    /// <summary>
    /// Represents a node in a Proxmox VE cluster.
    /// </summary>
    public class ProxmoxClusterNode
    {
        /// <summary>
        /// Gets or sets the node name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the node ID.
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the node IP address.
        /// </summary>
        [JsonProperty("ip")]
        public string IP { get; set; }

        /// <summary>
        /// Gets or sets the node online status.
        /// </summary>
        [JsonProperty("online")]
        public bool Online { get; set; }

        /// <summary>
        /// Gets or sets the node type.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }
    }

    /// <summary>
    /// Represents the quorum information for a Proxmox VE cluster.
    /// </summary>
    public class ProxmoxClusterQuorum
    {
        /// <summary>
        /// Gets or sets the quorum status.
        /// </summary>
        [JsonProperty("quorate")]
        public bool Quorate { get; set; }

        /// <summary>
        /// Gets or sets the number of nodes.
        /// </summary>
        [JsonProperty("nodes")]
        public int Nodes { get; set; }

        /// <summary>
        /// Gets or sets the expected votes.
        /// </summary>
        [JsonProperty("expected_votes")]
        public int ExpectedVotes { get; set; }

        /// <summary>
        /// Gets or sets the number of votes needed for quorum.
        /// </summary>
        [JsonProperty("quorum")]
        public int Quorum { get; set; }
    }
}
