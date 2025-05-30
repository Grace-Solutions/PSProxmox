using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PSProxmox.Models
{
    /// <summary>
    /// Represents a virtual machine in Proxmox VE.
    /// </summary>
    public class ProxmoxVM
    {
        /// <summary>
        /// Gets or sets the VM ID.
        /// </summary>
        [JsonProperty("vmid")]
        public int VMID { get; set; }

        /// <summary>
        /// Gets or sets the VM name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the VM status.
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the VM CPU count.
        /// </summary>
        [JsonProperty("cpus")]
        public int CPUs { get; set; }

        /// <summary>
        /// Gets or sets the VM memory in bytes.
        /// </summary>
        [JsonProperty("maxmem")]
        public long MaxMemory { get; set; }

        /// <summary>
        /// Gets or sets the VM disk size in bytes.
        /// </summary>
        [JsonProperty("maxdisk")]
        public long MaxDisk { get; set; }

        /// <summary>
        /// Gets or sets the VM uptime in seconds.
        /// </summary>
        [JsonProperty("uptime")]
        public long Uptime { get; set; }

        /// <summary>
        /// Gets or sets the VM node.
        /// </summary>
        [JsonProperty("node")]
        public string Node { get; set; }

        /// <summary>
        /// Gets or sets the VM type (qemu, lxc, etc.).
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the VM template flag.
        /// </summary>
        [JsonProperty("template")]
        public int Template { get; set; }

        /// <summary>
        /// Gets or sets the VM network interfaces.
        /// </summary>
        [JsonProperty("netif")]
        public string NetIf { get; set; }

        /// <summary>
        /// Gets or sets the VM tags.
        /// </summary>
        [JsonProperty("tags")]
        public string Tags { get; set; }

        /// <summary>
        /// Gets or sets the VM guest agent information.
        /// </summary>
        [JsonIgnore]
        public ProxmoxVMGuestAgent GuestAgent { get; set; }

        /// <summary>
        /// Gets a value indicating whether the VM is a template.
        /// </summary>
        [JsonIgnore]
        public bool IsTemplate => Template == 1;

        /// <summary>
        /// Gets a value indicating whether the VM is running.
        /// </summary>
        [JsonIgnore]
        public bool IsRunning => Status == "running";

        /// <summary>
        /// Gets the VM memory in megabytes.
        /// </summary>
        [JsonIgnore]
        public int MemoryMB => (int)(MaxMemory / 1024 / 1024);

        /// <summary>
        /// Gets the VM disk size in gigabytes.
        /// </summary>
        [JsonIgnore]
        public int DiskGB => (int)(MaxDisk / 1024 / 1024 / 1024);

        /// <summary>
        /// Gets the VM uptime as a TimeSpan.
        /// </summary>
        [JsonIgnore]
        public TimeSpan UptimeSpan => TimeSpan.FromSeconds(Uptime);
    }
}
