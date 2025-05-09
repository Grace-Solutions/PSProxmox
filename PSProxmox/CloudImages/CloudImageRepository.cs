using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PSProxmox.CloudImages
{
    /// <summary>
    /// Represents a cloud image repository
    /// </summary>
    public class CloudImageRepository
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly string _cacheDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PSProxmox", "CloudImageCache");
        private static readonly string _metadataFile = Path.Combine(_cacheDirectory, "metadata.json");
        private static readonly TimeSpan _cacheExpiration = TimeSpan.FromDays(1);

        /// <summary>
        /// Gets the list of supported distributions
        /// </summary>
        public static IEnumerable<string> SupportedDistributions => new[] { "ubuntu", "debian" };

        /// <summary>
        /// Gets the list of available cloud images for a specific distribution
        /// </summary>
        /// <param name="distribution">The distribution name (e.g., "ubuntu", "debian")</param>
        /// <returns>A list of available cloud images</returns>
        public static async Task<IEnumerable<CloudImage>> GetAvailableImagesAsync(string distribution)
        {
            EnsureCacheDirectoryExists();

            var metadata = await GetMetadataAsync();
            var distroMetadata = metadata.ContainsKey(distribution) ? metadata[distribution] : new Dictionary<string, CloudImageMetadata>();

            // Check if we need to refresh the cache
            bool needsRefresh = !distroMetadata.Any() ||
                                distroMetadata.Values.Any(m => DateTime.UtcNow - m.LastUpdated > _cacheExpiration);

            if (needsRefresh)
            {
                distroMetadata = await FetchDistributionMetadataAsync(distribution);
                metadata[distribution] = distroMetadata;
                await SaveMetadataAsync(metadata);
            }

            return distroMetadata.Values.SelectMany(m => m.Images);
        }

        /// <summary>
        /// Gets a specific cloud image by distribution, release, and variant
        /// </summary>
        /// <param name="distribution">The distribution name (e.g., "ubuntu", "debian")</param>
        /// <param name="release">The release version (e.g., "22.04", "11")</param>
        /// <param name="variant">The image variant (e.g., "server", "minimal")</param>
        /// <returns>The cloud image if found, null otherwise</returns>
        public static async Task<CloudImage> GetImageAsync(string distribution, string release, string variant = null)
        {
            var images = await GetAvailableImagesAsync(distribution);
            return images.FirstOrDefault(i =>
                i.Release == release &&
                (string.IsNullOrEmpty(variant) || i.Variant == variant));
        }

        private static async Task<Dictionary<string, Dictionary<string, CloudImageMetadata>>> GetMetadataAsync()
        {
            if (File.Exists(_metadataFile))
            {
                try
                {
                    var json = File.ReadAllText(_metadataFile);
                    return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, CloudImageMetadata>>>(json);
                }
                catch
                {
                    // If there's an error reading the file, return an empty dictionary
                }
            }

            return new Dictionary<string, Dictionary<string, CloudImageMetadata>>(StringComparer.OrdinalIgnoreCase);
        }

        private static async Task SaveMetadataAsync(Dictionary<string, Dictionary<string, CloudImageMetadata>> metadata)
        {
            var json = JsonConvert.SerializeObject(metadata, Formatting.Indented);
            File.WriteAllText(_metadataFile, json);
        }

        private static async Task<Dictionary<string, CloudImageMetadata>> FetchDistributionMetadataAsync(string distribution)
        {
            var result = new Dictionary<string, CloudImageMetadata>(StringComparer.OrdinalIgnoreCase);

            switch (distribution.ToLowerInvariant())
            {
                case "ubuntu":
                    await FetchUbuntuMetadataAsync(result);
                    break;
                case "debian":
                    await FetchDebianMetadataAsync(result);
                    break;
            }

            return result;
        }

        private static async Task FetchUbuntuMetadataAsync(Dictionary<string, CloudImageMetadata> result)
        {
            // Ubuntu cloud images are available at https://cloud-images.ubuntu.com/
            var releasesUrl = "https://cloud-images.ubuntu.com/releases/";
            var dailyUrl = "https://cloud-images.ubuntu.com/daily/";

            var releasesHtml = await _httpClient.GetStringAsync(releasesUrl);
            var dailyHtml = await _httpClient.GetStringAsync(dailyUrl);

            // Parse the HTML to extract available releases
            var releaseRegex = new Regex(@"<a href=""([0-9\.]+)/""", RegexOptions.Compiled);
            var matches = releaseRegex.Matches(releasesHtml);
            var releases = new List<string>();
            foreach (Match match in matches)
            {
                releases.Add(match.Groups[1].Value);
            }
            releases = releases.Distinct().ToList();

            foreach (var release in releases)
            {
                var releaseUrl = $"{releasesUrl}{release}/release/";
                try
                {
                    var releaseHtml = await _httpClient.GetStringAsync(releaseUrl);

                    // Look for server cloud images
                    var serverImageRegex = new Regex(@"<a href=""(ubuntu-" + release + @"-server-cloudimg-amd64(?:\.img|\.qcow2|\.tar\.gz))""", RegexOptions.Compiled);
                    var serverMatches = serverImageRegex.Matches(releaseHtml);
                    var serverImages = new List<string>();
                    foreach (Match match in serverMatches)
                    {
                        serverImages.Add(match.Groups[1].Value);
                    }

                    if (serverImages.Any())
                    {
                        var metadata = new CloudImageMetadata
                        {
                            LastUpdated = DateTime.UtcNow,
                            Images = new List<CloudImage>()
                        };

                        foreach (var image in serverImages)
                        {
                            var extension = Path.GetExtension(image).ToLowerInvariant();
                            if (extension == ".img" || extension == ".qcow2")
                            {
                                metadata.Images.Add(new CloudImage
                                {
                                    Distribution = "ubuntu",
                                    Release = release,
                                    Variant = "server",
                                    Url = $"{releaseUrl}{image}",
                                    Filename = image,
                                    Format = extension == ".img" ? "raw" : "qcow2"
                                });
                            }
                        }

                        result[release] = metadata;
                    }
                }
                catch
                {
                    // Skip this release if there's an error
                }
            }
        }

        private static async Task FetchDebianMetadataAsync(Dictionary<string, CloudImageMetadata> result)
        {
            // Debian cloud images are available at https://cloud.debian.org/images/cloud/
            var baseUrl = "https://cloud.debian.org/images/cloud/";

            var html = await _httpClient.GetStringAsync(baseUrl);

            // Parse the HTML to extract available releases
            var releaseRegex = new Regex(@"<a href=""([^/]+)/""", RegexOptions.Compiled);
            var matches = releaseRegex.Matches(html);
            var releases = new List<string>();
            foreach (Match match in matches)
            {
                var value = match.Groups[1].Value;
                if (!value.Contains(".."))
                {
                    releases.Add(value);
                }
            }

            foreach (var release in releases)
            {
                var releaseUrl = $"{baseUrl}{release}/latest/";
                try
                {
                    var releaseHtml = await _httpClient.GetStringAsync(releaseUrl);

                    // Look for generic cloud images
                    var imageRegex = new Regex(@"<a href=""(debian-[0-9]+-generic-amd64(?:\.qcow2|\.raw))""", RegexOptions.Compiled);
                    var imageMatches = imageRegex.Matches(releaseHtml);
                    var images = new List<string>();
                    foreach (Match match in imageMatches)
                    {
                        images.Add(match.Groups[1].Value);
                    }

                    if (images.Any())
                    {
                        var metadata = new CloudImageMetadata
                        {
                            LastUpdated = DateTime.UtcNow,
                            Images = new List<CloudImage>()
                        };

                        foreach (var image in images)
                        {
                            var extension = Path.GetExtension(image).ToLowerInvariant();
                            metadata.Images.Add(new CloudImage
                            {
                                Distribution = "debian",
                                Release = release,
                                Variant = "generic",
                                Url = $"{releaseUrl}{image}",
                                Filename = image,
                                Format = extension == ".raw" ? "raw" : "qcow2"
                            });
                        }

                        result[release] = metadata;
                    }
                }
                catch
                {
                    // Skip this release if there's an error
                }
            }
        }

        private static void EnsureCacheDirectoryExists()
        {
            if (!Directory.Exists(_cacheDirectory))
            {
                Directory.CreateDirectory(_cacheDirectory);
            }
        }
    }

    /// <summary>
    /// Represents metadata for cloud images
    /// </summary>
    public class CloudImageMetadata
    {
        /// <summary>
        /// Gets or sets the last updated time
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the list of cloud images
        /// </summary>
        public List<CloudImage> Images { get; set; }
    }

    /// <summary>
    /// Represents a cloud image
    /// </summary>
    public class CloudImage
    {
        /// <summary>
        /// Gets or sets the distribution name
        /// </summary>
        public string Distribution { get; set; }

        /// <summary>
        /// Gets or sets the release version
        /// </summary>
        public string Release { get; set; }

        /// <summary>
        /// Gets or sets the image variant
        /// </summary>
        public string Variant { get; set; }

        /// <summary>
        /// Gets or sets the image URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the image filename
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Gets or sets the image format
        /// </summary>
        public string Format { get; set; }
    }
}
