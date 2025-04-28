using Newtonsoft.Json;

namespace PSProxmox.Models
{
    /// <summary>
    /// Represents a storage in Proxmox VE.
    /// </summary>
    public class ProxmoxStorage
    {
        /// <summary>
        /// Gets or sets the storage name.
        /// </summary>
        [JsonProperty("storage")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the storage type.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the storage content types.
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the storage path.
        /// </summary>
        [JsonProperty("path")]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the storage node.
        /// </summary>
        [JsonProperty("node")]
        public string Node { get; set; }

        /// <summary>
        /// Gets or sets the storage used space in bytes.
        /// </summary>
        [JsonProperty("used")]
        public long Used { get; set; }

        /// <summary>
        /// Gets or sets the storage available space in bytes.
        /// </summary>
        [JsonProperty("avail")]
        public long Available { get; set; }

        /// <summary>
        /// Gets or sets the storage total space in bytes.
        /// </summary>
        [JsonProperty("total")]
        public long Total { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the storage is enabled.
        /// </summary>
        [JsonProperty("enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the storage is shared.
        /// </summary>
        [JsonProperty("shared")]
        public bool Shared { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the storage is active.
        /// </summary>
        [JsonProperty("active")]
        public bool Active { get; set; }

        /// <summary>
        /// Gets the storage used space in gigabytes.
        /// </summary>
        [JsonIgnore]
        public double UsedGB => Used / 1024.0 / 1024.0 / 1024.0;

        /// <summary>
        /// Gets the storage available space in gigabytes.
        /// </summary>
        [JsonIgnore]
        public double AvailableGB => Available / 1024.0 / 1024.0 / 1024.0;

        /// <summary>
        /// Gets the storage total space in gigabytes.
        /// </summary>
        [JsonIgnore]
        public double TotalGB => Total / 1024.0 / 1024.0 / 1024.0;

        /// <summary>
        /// Gets the storage usage percentage.
        /// </summary>
        [JsonIgnore]
        public double UsagePercent => Total > 0 ? (double)Used / Total * 100 : 0;
    }
}
