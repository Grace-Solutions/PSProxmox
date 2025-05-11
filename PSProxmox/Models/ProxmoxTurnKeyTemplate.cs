using System;
using Newtonsoft.Json;

namespace PSProxmox.Models
{
    /// <summary>
    /// Represents a TurnKey Linux template for Proxmox LXC containers
    /// </summary>
    public class ProxmoxTurnKeyTemplate
    {
        /// <summary>
        /// Gets or sets the template name
        /// </summary>
        [JsonProperty("template")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the template title
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the template description
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the template section
        /// </summary>
        [JsonProperty("section")]
        public string Section { get; set; }

        /// <summary>
        /// Gets or sets the template OS
        /// </summary>
        [JsonProperty("os")]
        public string OS { get; set; }

        /// <summary>
        /// Gets or sets the template version
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the template package
        /// </summary>
        [JsonProperty("package")]
        public string Package { get; set; }

        /// <summary>
        /// Gets or sets the template source
        /// </summary>
        [JsonProperty("source")]
        public string Source { get; set; }

        /// <summary>
        /// Gets or sets the template URL
        /// </summary>
        [JsonProperty("url")]
        public string URL { get; set; }

        /// <summary>
        /// Gets or sets the template size in bytes
        /// </summary>
        [JsonProperty("size")]
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets the template MD5 checksum
        /// </summary>
        [JsonProperty("md5sum")]
        public string MD5Sum { get; set; }

        /// <summary>
        /// Gets or sets the template SHA1 checksum
        /// </summary>
        [JsonProperty("sha1sum")]
        public string SHA1Sum { get; set; }

        /// <summary>
        /// Gets or sets the template SHA256 checksum
        /// </summary>
        [JsonProperty("sha256sum")]
        public string SHA256Sum { get; set; }

        /// <summary>
        /// Gets or sets the template architecture
        /// </summary>
        [JsonProperty("architecture")]
        public string Architecture { get; set; }

        /// <summary>
        /// Gets or sets the template infopage URL
        /// </summary>
        [JsonProperty("infopage")]
        public string InfoPage { get; set; }

        /// <summary>
        /// Gets or sets the template download URL
        /// </summary>
        [JsonProperty("location")]
        public string Location { get; set; }

        /// <summary>
        /// Gets the template size in a human-readable format
        /// </summary>
        public string HumanSize
        {
            get
            {
                string[] sizes = { "B", "KB", "MB", "GB", "TB" };
                double len = Size;
                int order = 0;
                while (len >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    len = len / 1024;
                }
                return $"{len:0.##} {sizes[order]}";
            }
        }

        /// <summary>
        /// Gets the full template name including version
        /// </summary>
        public string FullName
        {
            get
            {
                return $"{Name}-{Version}";
            }
        }
    }
}
