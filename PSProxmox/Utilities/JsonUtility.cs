using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PSProxmox.Models;

namespace PSProxmox.Utilities
{
    /// <summary>
    /// Utility class for JSON operations.
    /// </summary>
    public static class JsonUtility
    {
        /// <summary>
        /// Deserializes a JSON string to an object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="json">The JSON string.</param>
        /// <returns>The deserialized object.</returns>
        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        /// <summary>
        /// Deserializes a Proxmox API response to an object of the specified type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="json">The JSON string.</param>
        /// <returns>The deserialized object.</returns>
        public static T DeserializeResponse<T>(string json)
        {
            var response = JsonConvert.DeserializeObject<ProxmoxResponse<T>>(json);
            return response.Data;
        }

        /// <summary>
        /// Serializes an object to a JSON string.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The JSON string.</returns>
        public static string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        /// <summary>
        /// Converts a JSON string to a JObject.
        /// </summary>
        /// <param name="json">The JSON string.</param>
        /// <returns>The JObject.</returns>
        public static JObject ToJObject(string json)
        {
            return JObject.Parse(json);
        }

        /// <summary>
        /// Converts a JSON string to a JArray.
        /// </summary>
        /// <param name="json">The JSON string.</param>
        /// <returns>The JArray.</returns>
        public static JArray ToJArray(string json)
        {
            return JArray.Parse(json);
        }
    }
}
