using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;
using PSProxmox.CloudImages;
using PSProxmox.Models;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Gets available cloud images from repositories.</para>
    /// <para type="description">The Get-ProxmoxCloudImage cmdlet gets available cloud images from repositories.</para>
    /// <para type="description">You can filter the results by distribution, release, and variant.</para>
    /// </summary>
    /// <example>
    ///   <para>Get all available cloud images</para>
    ///   <code>Get-ProxmoxCloudImage</code>
    /// </example>
    /// <example>
    ///   <para>Get Ubuntu cloud images</para>
    ///   <code>Get-ProxmoxCloudImage -Distribution "ubuntu"</code>
    /// </example>
    /// <example>
    ///   <para>Get Ubuntu 22.04 cloud images</para>
    ///   <code>Get-ProxmoxCloudImage -Distribution "ubuntu" -Release "22.04"</code>
    /// </example>
    /// <example>
    ///   <para>Get Ubuntu 22.04 server cloud image and download it</para>
    ///   <code>Get-ProxmoxCloudImage -Distribution "ubuntu" -Release "22.04" -Variant "server" | Save-ProxmoxCloudImage</code>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "ProxmoxCloudImage")]
    [OutputType(typeof(CloudImage))]
    public class GetProxmoxCloudImageCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The distribution to filter by (e.g., "ubuntu", "debian").</para>
        /// </summary>
        [Parameter(Mandatory = false, Position = 0)]
        [ValidateSet("ubuntu", "debian")]
        public string Distribution { get; set; }

        /// <summary>
        /// <para type="description">The release version to filter by (e.g., "22.04", "11").</para>
        /// </summary>
        [Parameter(Mandatory = false, Position = 1)]
        public string Release { get; set; }

        /// <summary>
        /// <para type="description">The image variant to filter by (e.g., "server", "minimal").</para>
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public string Variant { get; set; }

        /// <summary>
        /// <para type="description">Force refresh of the cloud image cache.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                if (string.IsNullOrEmpty(Distribution))
                {
                    // Get images for all supported distributions
                    foreach (var distro in CloudImageRepository.SupportedDistributions)
                    {
                        GetImagesForDistribution(distro);
                    }
                }
                else
                {
                    // Get images for the specified distribution
                    GetImagesForDistribution(Distribution);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(
                    ex,
                    "GetProxmoxCloudImageError",
                    ErrorCategory.NotSpecified,
                    null));
            }
        }

        private void GetImagesForDistribution(string distribution)
        {
            var task = Task.Run(async () =>
            {
                IEnumerable<CloudImage> images;

                if (!string.IsNullOrEmpty(Release) && !string.IsNullOrEmpty(Variant))
                {
                    // Get a specific image
                    var image = await CloudImageRepository.GetImageAsync(distribution, Release, Variant);
                    images = image != null ? new[] { image } : Array.Empty<CloudImage>();
                }
                else if (!string.IsNullOrEmpty(Release))
                {
                    // Get images for a specific release
                    var allImages = await CloudImageRepository.GetAvailableImagesAsync(distribution);
                    images = allImages.Where(i => i.Release == Release);
                }
                else
                {
                    // Get all images for the distribution
                    images = await CloudImageRepository.GetAvailableImagesAsync(distribution);
                }

                return images;
            });

            task.Wait();

            foreach (var image in task.Result)
            {
                WriteObject(image);
            }
        }
    }
}
