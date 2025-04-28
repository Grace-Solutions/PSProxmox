using Newtonsoft.Json;

namespace PSProxmox.Models
{
    /// <summary>
    /// Represents a response from the Proxmox VE API.
    /// </summary>
    /// <typeparam name="T">The type of data in the response.</typeparam>
    public class ProxmoxResponse<T>
    {
        /// <summary>
        /// Gets or sets the data in the response.
        /// </summary>
        [JsonProperty("data")]
        public T Data { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the request was successful.
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}
