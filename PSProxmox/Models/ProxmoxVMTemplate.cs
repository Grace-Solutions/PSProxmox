using Newtonsoft.Json;
using System;

namespace PSProxmox.Models
{
    /// <summary>
    /// Represents a virtual machine template in Proxmox VE.
    /// </summary>
    public class ProxmoxVMTemplate
    {
        /// <summary>
        /// Gets or sets the template ID.
        /// </summary>
        [JsonProperty("vmid")]
        public int VMID { get; set; }

        /// <summary>
        /// Gets or sets the template name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the template node.
        /// </summary>
        [JsonProperty("node")]
        public string Node { get; set; }

        /// <summary>
        /// Gets or sets the template CPU count.
        /// </summary>
        [JsonProperty("cpus")]
        public int CPUs { get; set; }

        /// <summary>
        /// Gets or sets the template memory in bytes.
        /// </summary>
        [JsonProperty("maxmem")]
        public long MaxMemory { get; set; }

        /// <summary>
        /// Gets or sets the template disk size in bytes.
        /// </summary>
        [JsonProperty("maxdisk")]
        public long MaxDisk { get; set; }

        /// <summary>
        /// Gets or sets the template type (qemu, lxc, etc.).
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Gets or sets the template description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the template creation time.
        /// </summary>
        [JsonProperty("ctime")]
        public long CreationTime { get; set; }

        /// <summary>
        /// Gets or sets the template tags.
        /// </summary>
        [JsonProperty("tags")]
        public string Tags { get; set; }

        /// <summary>
        /// Gets the template memory in megabytes.
        /// </summary>
        [JsonIgnore]
        public int MemoryMB => (int)(MaxMemory / 1024 / 1024);

        /// <summary>
        /// Gets the template disk size in gigabytes.
        /// </summary>
        [JsonIgnore]
        public int DiskGB => (int)(MaxDisk / 1024 / 1024 / 1024);

        /// <summary>
        /// Gets the template creation date.
        /// </summary>
        [JsonIgnore]
        public DateTime CreationDate => DateTimeOffset.FromUnixTimeSeconds(CreationTime).DateTime;

        /// <summary>
        /// Creates a new instance of the <see cref="ProxmoxVMTemplate"/> class from a <see cref="ProxmoxVM"/> object.
        /// </summary>
        /// <param name="vm">The VM object.</param>
        /// <returns>A new template object.</returns>
        public static ProxmoxVMTemplate FromVM(ProxmoxVM vm)
        {
            return new ProxmoxVMTemplate
            {
                VMID = vm.VMID,
                Name = vm.Name,
                Node = vm.Node,
                CPUs = vm.CPUs,
                MaxMemory = vm.MaxMemory,
                MaxDisk = vm.MaxDisk,
                Type = vm.Type,
                Tags = vm.Tags,
                CreationTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
        }
    }
}
