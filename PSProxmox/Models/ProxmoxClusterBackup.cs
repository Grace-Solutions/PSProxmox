using Newtonsoft.Json;
using System;

namespace PSProxmox.Models
{
    /// <summary>
    /// Represents a cluster backup in Proxmox VE.
    /// </summary>
    public class ProxmoxClusterBackup
    {
        /// <summary>
        /// Gets or sets the backup ID.
        /// </summary>
        [JsonProperty("backup-id")]
        public string BackupID { get; set; }

        /// <summary>
        /// Gets or sets the backup time.
        /// </summary>
        [JsonProperty("time")]
        public long Time { get; set; }

        /// <summary>
        /// Gets or sets the backup type.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the backup version.
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the backup size.
        /// </summary>
        [JsonProperty("size")]
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets the backup compression.
        /// </summary>
        [JsonProperty("compression")]
        public string Compression { get; set; }

        /// <summary>
        /// Gets or sets the backup node.
        /// </summary>
        [JsonProperty("node")]
        public string Node { get; set; }

        /// <summary>
        /// Gets or sets the backup file path.
        /// </summary>
        [JsonProperty("path")]
        public string Path { get; set; }

        /// <summary>
        /// Gets the backup date.
        /// </summary>
        [JsonIgnore]
        public DateTime Date => DateTimeOffset.FromUnixTimeSeconds(Time).DateTime;

        /// <summary>
        /// Gets the backup size in megabytes.
        /// </summary>
        [JsonIgnore]
        public double SizeMB => Size / 1024.0 / 1024.0;

        /// <summary>
        /// Gets the backup size in gigabytes.
        /// </summary>
        [JsonIgnore]
        public double SizeGB => Size / 1024.0 / 1024.0 / 1024.0;
    }
}
