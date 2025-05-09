using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using PSProxmox.CloudImages;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Customizes a cloud image.</para>
    /// <para type="description">The Invoke-ProxmoxCloudImageCustomization cmdlet customizes a cloud image by resizing it, adding packages, or running commands or scripts.</para>
    /// </summary>
    /// <example>
    ///   <para>Resize a cloud image to 20GB</para>
    ///   <code>Invoke-ProxmoxCloudImageCustomization -ImagePath "C:\Images\ubuntu-22.04-server-cloudimg-amd64.img" -Resize 20</code>
    /// </example>
    /// <example>
    ///   <para>Convert a cloud image to qcow2 format</para>
    ///   <code>Invoke-ProxmoxCloudImageCustomization -ImagePath "C:\Images\ubuntu-22.04-server-cloudimg-amd64.img" -ConvertTo "qcow2"</code>
    /// </example>
    [Cmdlet(VerbsLifecycle.Invoke, "ProxmoxCloudImageCustomization")]
    [OutputType(typeof(string))]
    public class InvokeProxmoxCloudImageCustomizationCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The path to the cloud image file.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string ImagePath { get; set; }

        /// <summary>
        /// <para type="description">The new size of the image in GB.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int? Resize { get; set; }

        /// <summary>
        /// <para type="description">The format to convert the image to (e.g., "qcow2", "raw").</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        [ValidateSet("qcow2", "raw")]
        public string ConvertTo { get; set; }

        /// <summary>
        /// <para type="description">The packages to install in the image.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string[] Packages { get; set; }

        /// <summary>
        /// <para type="description">The commands to run in the image.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string[] Commands { get; set; }

        /// <summary>
        /// <para type="description">The scripts to run in the image.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string[] Scripts { get; set; }

        /// <summary>
        /// <para type="description">The output path for the customized image. If not specified, the original image will be modified.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string OutputPath { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                if (!File.Exists(ImagePath))
                {
                    throw new FileNotFoundException("Image file not found", ImagePath);
                }

                string resultPath = ImagePath;

                // Resize the image if requested
                if (Resize.HasValue)
                {
                    WriteVerbose($"Resizing image to {Resize.Value}GB...");
                    
                    var progressRecord = new ProgressRecord(
                        0,
                        $"Resizing image to {Resize.Value}GB",
                        "0% complete");
                    
                    progressRecord.PercentComplete = 0;
                    WriteProgress(progressRecord);
                    
                    resultPath = CloudImageCustomizer.ResizeImage(
                        resultPath,
                        Resize.Value,
                        message =>
                        {
                            progressRecord.StatusDescription = message;
                            progressRecord.PercentComplete = 50;
                            WriteProgress(progressRecord);
                        });
                    
                    progressRecord.PercentComplete = 100;
                    progressRecord.StatusDescription = "Resize complete";
                    progressRecord.RecordType = ProgressRecordType.Completed;
                    WriteProgress(progressRecord);
                    
                    WriteVerbose("Image resized successfully");
                }

                // Convert the image if requested
                if (!string.IsNullOrEmpty(ConvertTo))
                {
                    WriteVerbose($"Converting image to {ConvertTo}...");
                    
                    var progressRecord = new ProgressRecord(
                        1,
                        $"Converting image to {ConvertTo}",
                        "0% complete");
                    
                    progressRecord.PercentComplete = 0;
                    WriteProgress(progressRecord);
                    
                    resultPath = CloudImageCustomizer.ConvertImage(
                        resultPath,
                        ConvertTo,
                        message =>
                        {
                            progressRecord.StatusDescription = message;
                            progressRecord.PercentComplete = 50;
                            WriteProgress(progressRecord);
                        });
                    
                    progressRecord.PercentComplete = 100;
                    progressRecord.StatusDescription = "Conversion complete";
                    progressRecord.RecordType = ProgressRecordType.Completed;
                    WriteProgress(progressRecord);
                    
                    WriteVerbose("Image converted successfully");
                }

                // Customize the image if requested
                if ((Packages != null && Packages.Length > 0) ||
                    (Commands != null && Commands.Length > 0) ||
                    (Scripts != null && Scripts.Length > 0))
                {
                    WriteVerbose("Customizing image...");
                    
                    var progressRecord = new ProgressRecord(
                        2,
                        "Customizing image",
                        "0% complete");
                    
                    progressRecord.PercentComplete = 0;
                    WriteProgress(progressRecord);
                    
                    resultPath = CloudImageCustomizer.CustomizeImage(
                        resultPath,
                        Packages,
                        Commands,
                        Scripts,
                        message =>
                        {
                            progressRecord.StatusDescription = message;
                            progressRecord.PercentComplete = 50;
                            WriteProgress(progressRecord);
                        });
                    
                    progressRecord.PercentComplete = 100;
                    progressRecord.StatusDescription = "Customization complete";
                    progressRecord.RecordType = ProgressRecordType.Completed;
                    WriteProgress(progressRecord);
                    
                    WriteVerbose("Image customized successfully");
                }

                // Copy to output path if specified
                if (!string.IsNullOrEmpty(OutputPath) && resultPath != OutputPath)
                {
                    WriteVerbose($"Copying image to {OutputPath}...");
                    
                    var outputDirectory = Path.GetDirectoryName(OutputPath);
                    if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                    {
                        Directory.CreateDirectory(outputDirectory);
                    }
                    
                    File.Copy(resultPath, OutputPath, true);
                    resultPath = OutputPath;
                    
                    WriteVerbose("Image copied successfully");
                }

                WriteObject(resultPath);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(
                    ex,
                    "InvokeProxmoxCloudImageCustomizationError",
                    ErrorCategory.NotSpecified,
                    null));
            }
        }
    }
}
