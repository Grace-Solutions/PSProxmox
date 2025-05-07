using System;
using Newtonsoft.Json;

namespace PSProxmox.Models
{
    /// <summary>
    /// Represents SMBIOS settings for a Proxmox VM.
    /// </summary>
    public class ProxmoxVMSMBIOS
    {
        /// <summary>
        /// Gets or sets the manufacturer information.
        /// </summary>
        [JsonProperty("manufacturer")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// Gets or sets the product information.
        /// </summary>
        [JsonProperty("product")]
        public string Product { get; set; }

        /// <summary>
        /// Gets or sets the version information.
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the serial number.
        /// </summary>
        [JsonProperty("serial")]
        public string Serial { get; set; }

        /// <summary>
        /// Gets or sets the system family.
        /// </summary>
        [JsonProperty("family")]
        public string Family { get; set; }

        /// <summary>
        /// Gets or sets the system UUID.
        /// </summary>
        [JsonProperty("uuid")]
        public string UUID { get; set; }

        /// <summary>
        /// Converts the SMBIOS settings to a Proxmox API compatible string.
        /// </summary>
        /// <returns>A string representation of the SMBIOS settings.</returns>
        public string ToProxmoxString()
        {
            var parts = new System.Collections.Generic.List<string>();

            if (!string.IsNullOrEmpty(Manufacturer))
                parts.Add($"manufacturer={Manufacturer}");

            if (!string.IsNullOrEmpty(Product))
                parts.Add($"product={Product}");

            if (!string.IsNullOrEmpty(Version))
                parts.Add($"version={Version}");

            if (!string.IsNullOrEmpty(Serial))
                parts.Add($"serial={Serial}");

            if (!string.IsNullOrEmpty(Family))
                parts.Add($"family={Family}");

            if (!string.IsNullOrEmpty(UUID))
                parts.Add($"uuid={UUID}");

            return string.Join(",", parts);
        }

        /// <summary>
        /// Parses a Proxmox API SMBIOS string into a ProxmoxVMSMBIOS object.
        /// </summary>
        /// <param name="smbiosString">The SMBIOS string from the Proxmox API.</param>
        /// <returns>A ProxmoxVMSMBIOS object.</returns>
        public static ProxmoxVMSMBIOS FromProxmoxString(string smbiosString)
        {
            if (string.IsNullOrEmpty(smbiosString))
                return new ProxmoxVMSMBIOS();

            // If the string is just "1", it means SMBIOS is enabled but no specific values are set
            if (smbiosString == "1")
                return new ProxmoxVMSMBIOS();

            var result = new ProxmoxVMSMBIOS();
            var parts = smbiosString.Split(',');

            foreach (var part in parts)
            {
                var keyValue = part.Split('=');
                if (keyValue.Length != 2)
                    continue;

                var key = keyValue[0].Trim();
                var value = keyValue[1].Trim();

                switch (key.ToLowerInvariant())
                {
                    case "manufacturer":
                        result.Manufacturer = value;
                        break;
                    case "product":
                        result.Product = value;
                        break;
                    case "version":
                        result.Version = value;
                        break;
                    case "serial":
                        result.Serial = value;
                        break;
                    case "family":
                        result.Family = value;
                        break;
                    case "uuid":
                        result.UUID = value;
                        break;
                }
            }

            return result;
        }
    }
}
