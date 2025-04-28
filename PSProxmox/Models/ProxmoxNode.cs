using System;
using Newtonsoft.Json;

namespace PSProxmox.Models
{
    /// <summary>
    /// Represents a node in a Proxmox VE cluster.
    /// </summary>
    public class ProxmoxNode
    {
        /// <summary>
        /// Gets or sets the node name.
        /// </summary>
        [JsonProperty("node")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the node status.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the node uptime in seconds.
        /// </summary>
        [JsonProperty("uptime")]
        public long Uptime { get; set; }

        /// <summary>
        /// Gets or sets the node CPU usage.
        /// </summary>
        [JsonProperty("cpu")]
        public double CPU { get; set; }

        /// <summary>
        /// Gets or sets the node memory usage.
        /// </summary>
        [JsonProperty("mem")]
        public long Memory { get; set; }

        /// <summary>
        /// Gets or sets the node total memory.
        /// </summary>
        [JsonProperty("maxmem")]
        public long MaxMemory { get; set; }

        /// <summary>
        /// Gets or sets the node disk usage.
        /// </summary>
        [JsonProperty("disk")]
        public long Disk { get; set; }

        /// <summary>
        /// Gets or sets the node total disk space.
        /// </summary>
        [JsonProperty("maxdisk")]
        public long MaxDisk { get; set; }

        /// <summary>
        /// Gets or sets the node SSL fingerprint.
        /// </summary>
        [JsonProperty("ssl_fingerprint")]
        public string SSLFingerprint { get; set; }

        /// <summary>
        /// Gets or sets the node IP address.
        /// </summary>
        [JsonProperty("ip")]
        public string IP { get; set; }

        /// <summary>
        /// Gets or sets the node level.
        /// </summary>
        [JsonProperty("level")]
        public string Level { get; set; }

        /// <summary>
        /// Gets or sets the node ID.
        /// </summary>
        [JsonProperty("id")]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the node type.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets a value indicating whether the node is online.
        /// </summary>
        [JsonIgnore]
        public bool IsOnline => Status == "online";

        /// <summary>
        /// Gets the node uptime as a TimeSpan.
        /// </summary>
        [JsonIgnore]
        public TimeSpan UptimeSpan => TimeSpan.FromSeconds(Uptime);

        /// <summary>
        /// Gets the node memory usage percentage.
        /// </summary>
        [JsonIgnore]
        public double MemoryUsagePercent => MaxMemory > 0 ? (double)Memory / MaxMemory * 100 : 0;

        /// <summary>
        /// Gets the node disk usage percentage.
        /// </summary>
        [JsonIgnore]
        public double DiskUsagePercent => MaxDisk > 0 ? (double)Disk / MaxDisk * 100 : 0;

        /// <summary>
        /// Gets the node memory in gigabytes.
        /// </summary>
        [JsonIgnore]
        public double MemoryGB => MaxMemory / 1024.0 / 1024.0 / 1024.0;

        /// <summary>
        /// Gets the node disk space in gigabytes.
        /// </summary>
        [JsonIgnore]
        public double DiskGB => MaxDisk / 1024.0 / 1024.0 / 1024.0;
    }
}
