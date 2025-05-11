using System;
using System.IO;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;
using PSProxmox.CloudImages;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Downloads a cloud image.</para>
    /// <para type="description">The Save-ProxmoxCloudImage cmdlet downloads a cloud image from a repository.</para>
    /// <para type="description">You can specify the distribution, release, and variant to download.</para>
    /// </summary>
    /// <example>
    ///   <para>Download an Ubuntu 22.04 server cloud image</para>
    ///   <code>Save-ProxmoxCloudImage -Distribution "ubuntu" -Release "22.04" -Variant "server"</code>
    /// </example>
    /// <example>
    ///   <para>Download a Debian 11 generic cloud image</para>
    ///   <code>Save-ProxmoxCloudImage -Distribution "debian" -Release "11" -Variant "generic"</code>
    /// </example>
    /// <example>
    ///   <para>Get Ubuntu 22.04 server cloud image and download it</para>
    ///   <code>Get-ProxmoxCloudImage -Distribution "ubuntu" -Release "22.04" -Variant "server" | Save-ProxmoxCloudImage</code>
    /// </example>
    /// <example>
    ///   <para>Download an Ubuntu 22.04 server cloud image to a specific location</para>
    ///   <code>Save-ProxmoxCloudImage -Distribution "ubuntu" -Release "22.04" -Variant "server" -OutputPath "C:\Images\ubuntu-22.04-server.img"</code>
    /// </example>
    [Cmdlet(VerbsData.Save, "ProxmoxCloudImage")]
    [OutputType(typeof(string))]
    public class SaveProxmoxCloudImageCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The distribution to download (e.g., "ubuntu", "debian").</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true, ParameterSetName = "ByProperties")]
        [ValidateSet("ubuntu", "debian")]
        public string Distribution { get; set; }

        /// <summary>
        /// <para type="description">The release version to download (e.g., "22.04", "11").</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = true, ParameterSetName = "ByProperties")]
        public string Release { get; set; }

        /// <summary>
        /// <para type="description">The image variant to download (e.g., "server", "minimal").</para>
        /// </summary>
        [Parameter(Mandatory = false, Position = 2, ValueFromPipelineByPropertyName = true, ParameterSetName = "ByProperties")]
        public string Variant { get; set; }

        /// <summary>
        /// <para type="description">The cloud image object to download.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "ByCloudImage")]
        public CloudImage CloudImage { get; set; }

        /// <summary>
        /// <para type="description">The output path where the image will be saved. If not specified, the image will be saved to the default download directory.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "ByProperties")]
        [Parameter(Mandatory = false, ParameterSetName = "ByCloudImage")]
        public string OutputPath { get; set; }

        /// <summary>
        /// <para type="description">Force download even if the image already exists.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "ByProperties")]
        [Parameter(Mandatory = false, ParameterSetName = "ByCloudImage")]
        public SwitchParameter Force { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var task = Task.Run(async () =>
                {
                    // Get the cloud image
                    CloudImage image;

                    if (ParameterSetName == "ByCloudImage")
                    {
                        // Use the provided CloudImage object
                        image = CloudImage;
                    }
                    else
                    {
                        // Get the cloud image from the repository
                        image = await CloudImageRepository.GetImageAsync(Distribution, Release, Variant);
                        if (image == null)
                        {
                            throw new Exception($"Cloud image not found for {Distribution} {Release} {Variant}");
                        }
                    }

                    // Create a cancellation token source
                    using (var cts = new CancellationTokenSource())
                    {
                        // Download the image with progress reporting
                        var progressRecord = new ProgressRecord(
                            0,
                            $"Downloading {image.Filename}",
                            "0% complete");

                        var imagePath = await CloudImageDownloader.DownloadImageAsync(
                            image,
                            (bytesRead, totalBytes) =>
                            {
                                if (totalBytes > 0)
                                {
                                    var percentComplete = (int)((double)bytesRead / totalBytes * 100);
                                    progressRecord.PercentComplete = percentComplete;
                                    progressRecord.StatusDescription = $"{percentComplete}% complete";
                                    progressRecord.CurrentOperation = $"Downloaded {FormatBytes(bytesRead)} of {FormatBytes(totalBytes)}";
                                    WriteProgress(progressRecord);
                                }
                                else
                                {
                                    progressRecord.StatusDescription = $"Downloaded {FormatBytes(bytesRead)}";
                                    progressRecord.CurrentOperation = "Size unknown";
                                    WriteProgress(progressRecord);
                                }
                            },
                            cts.Token);

                        // Complete the progress
                        progressRecord.RecordType = ProgressRecordType.Completed;
                        WriteProgress(progressRecord);

                        // Copy to output path if specified
                        if (!string.IsNullOrEmpty(OutputPath))
                        {
                            var outputDirectory = Path.GetDirectoryName(OutputPath);
                            if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                            {
                                Directory.CreateDirectory(outputDirectory);
                            }

                            File.Copy(imagePath, OutputPath, true);
                            return OutputPath;
                        }

                        return imagePath;
                    }
                });

                task.Wait();
                WriteObject(task.Result);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(
                    ex,
                    "SaveProxmoxCloudImageError",
                    ErrorCategory.NotSpecified,
                    null));
            }
        }

        private string FormatBytes(long bytes)
        {
            string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
            int counter = 0;
            decimal number = bytes;
            while (Math.Round(number / 1024) >= 1)
            {
                number = number / 1024;
                counter++;
            }
            return $"{number:n1} {suffixes[counter]}";
        }
    }
}
