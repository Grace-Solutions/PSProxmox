using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PSProxmox.Models
{
    /// <summary>
    /// Represents a Proxmox LXC container
    /// </summary>
    public class ProxmoxContainer
    {
        /// <summary>
        /// Gets or sets the container ID
        /// </summary>
        [JsonProperty("vmid")]
        public int CTID { get; set; }

        /// <summary>
        /// Gets or sets the container name
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the node name
        /// </summary>
        [JsonProperty("node")]
        public string Node { get; set; }

        /// <summary>
        /// Gets or sets the container status
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the container uptime in seconds
        /// </summary>
        [JsonProperty("uptime")]
        public long Uptime { get; set; }

        /// <summary>
        /// Gets or sets the container CPU usage
        /// </summary>
        [JsonProperty("cpu")]
        public double CPU { get; set; }

        /// <summary>
        /// Gets or sets the container memory usage in bytes
        /// </summary>
        [JsonProperty("mem")]
        public long Memory { get; set; }

        /// <summary>
        /// Gets or sets the container maximum memory in bytes
        /// </summary>
        [JsonProperty("maxmem")]
        public long MaxMemory { get; set; }

        /// <summary>
        /// Gets or sets the container disk usage in bytes
        /// </summary>
        [JsonProperty("disk")]
        public long Disk { get; set; }

        /// <summary>
        /// Gets or sets the container maximum disk size in bytes
        /// </summary>
        [JsonProperty("maxdisk")]
        public long MaxDisk { get; set; }

        /// <summary>
        /// Gets or sets the container swap usage in bytes
        /// </summary>
        [JsonProperty("swap")]
        public long Swap { get; set; }

        /// <summary>
        /// Gets or sets the container maximum swap in bytes
        /// </summary>
        [JsonProperty("maxswap")]
        public long MaxSwap { get; set; }

        /// <summary>
        /// Gets or sets the container network receive rate in bytes per second
        /// </summary>
        [JsonProperty("netin")]
        public long NetworkIn { get; set; }

        /// <summary>
        /// Gets or sets the container network transmit rate in bytes per second
        /// </summary>
        [JsonProperty("netout")]
        public long NetworkOut { get; set; }

        /// <summary>
        /// Gets or sets the container disk read rate in bytes per second
        /// </summary>
        [JsonProperty("diskread")]
        public long DiskRead { get; set; }

        /// <summary>
        /// Gets or sets the container disk write rate in bytes per second
        /// </summary>
        [JsonProperty("diskwrite")]
        public long DiskWrite { get; set; }

        /// <summary>
        /// Gets or sets the container template flag
        /// </summary>
        [JsonProperty("template")]
        public int Template { get; set; }

        /// <summary>
        /// Gets or sets the container description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the container OS type
        /// </summary>
        [JsonProperty("ostype")]
        public string OSType { get; set; }

        /// <summary>
        /// Gets or sets the container IP address
        /// </summary>
        [JsonProperty("ip")]
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets the container hostname
        /// </summary>
        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        /// <summary>
        /// Gets or sets the container creation time
        /// </summary>
        [JsonProperty("creation")]
        public long CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the container tags
        /// </summary>
        [JsonProperty("tags")]
        public string Tags { get; set; }

        /// <summary>
        /// Gets or sets the container lock
        /// </summary>
        [JsonProperty("lock")]
        public string Lock { get; set; }

        /// <summary>
        /// Gets or sets the container unprivileged flag
        /// </summary>
        [JsonProperty("unprivileged")]
        public int Unprivileged { get; set; }

        /// <summary>
        /// Gets or sets the container features
        /// </summary>
        [JsonProperty("features")]
        public string Features { get; set; }

        /// <summary>
        /// Gets or sets the container configuration
        /// </summary>
        [JsonProperty("config")]
        public Dictionary<string, object> Config { get; set; }

        /// <summary>
        /// Gets the container creation date
        /// </summary>
        public DateTime CreationDate
        {
            get
            {
                return DateTimeOffset.FromUnixTimeSeconds(CreationTime).DateTime;
            }
        }

        /// <summary>
        /// Gets the container uptime as a TimeSpan
        /// </summary>
        public TimeSpan UptimeSpan
        {
            get
            {
                return TimeSpan.FromSeconds(Uptime);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the container is running
        /// </summary>
        public bool IsRunning
        {
            get
            {
                return Status == "running";
            }
        }

        /// <summary>
        /// Gets a value indicating whether the container is a template
        /// </summary>
        public bool IsTemplate
        {
            get
            {
                return Template == 1;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the container is unprivileged
        /// </summary>
        public bool IsUnprivileged
        {
            get
            {
                return Unprivileged == 1;
            }
        }
    }
}
