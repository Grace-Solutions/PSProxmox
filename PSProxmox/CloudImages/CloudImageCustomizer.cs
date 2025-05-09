using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PSProxmox.CloudImages
{
    /// <summary>
    /// Provides functionality for customizing cloud images
    /// </summary>
    public class CloudImageCustomizer
    {
        /// <summary>
        /// Resizes a cloud image
        /// </summary>
        /// <param name="imagePath">The path to the image file</param>
        /// <param name="newSize">The new size in GB</param>
        /// <param name="progressAction">Action to report progress</param>
        /// <returns>The path to the resized image</returns>
        public static string ResizeImage(string imagePath, int newSize, Action<string> progressAction)
        {
            if (!File.Exists(imagePath))
                throw new FileNotFoundException("Image file not found", imagePath);
            
            // Ensure qemu-img is available
            EnsureQemuImgAvailable();
            
            // Create a temporary file for the resized image
            var tempPath = Path.Combine(Path.GetDirectoryName(imagePath), $"{Path.GetFileNameWithoutExtension(imagePath)}_resized{Path.GetExtension(imagePath)}");
            
            try
            {
                progressAction?.Invoke("Resizing image...");
                
                // Run qemu-img resize command
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "qemu-img",
                        Arguments = $"resize \"{imagePath}\" {newSize}G",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                
                process.Start();
                
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                
                process.WaitForExit();
                
                if (process.ExitCode != 0)
                {
                    throw new Exception($"Failed to resize image: {error}");
                }
                
                progressAction?.Invoke("Image resized successfully");
                
                return imagePath;
            }
            catch (Exception ex)
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);
                
                throw new Exception($"Failed to resize image: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Customizes a cloud image by mounting it and running commands
        /// </summary>
        /// <param name="imagePath">The path to the image file</param>
        /// <param name="packages">The packages to install</param>
        /// <param name="commands">The commands to run</param>
        /// <param name="scripts">The scripts to run</param>
        /// <param name="progressAction">Action to report progress</param>
        /// <returns>The path to the customized image</returns>
        public static string CustomizeImage(
            string imagePath, 
            IEnumerable<string> packages, 
            IEnumerable<string> commands, 
            IEnumerable<string> scripts, 
            Action<string> progressAction)
        {
            if (!File.Exists(imagePath))
                throw new FileNotFoundException("Image file not found", imagePath);
            
            // This is a complex operation that requires mounting the image, chrooting into it,
            // and running commands. This is platform-specific and requires additional tools.
            // For simplicity, we'll just show the concept here.
            
            progressAction?.Invoke("Customizing image is not implemented yet");
            
            // In a real implementation, we would:
            // 1. Convert the image to raw format if needed
            // 2. Mount the image using loop devices or similar
            // 3. Chroot into the mounted filesystem
            // 4. Install packages and run commands
            // 5. Unmount the image
            // 6. Convert back to the original format if needed
            
            return imagePath;
        }

        /// <summary>
        /// Converts a cloud image to a different format
        /// </summary>
        /// <param name="imagePath">The path to the image file</param>
        /// <param name="format">The target format (qcow2, raw, etc.)</param>
        /// <param name="progressAction">Action to report progress</param>
        /// <returns>The path to the converted image</returns>
        public static string ConvertImage(string imagePath, string format, Action<string> progressAction)
        {
            if (!File.Exists(imagePath))
                throw new FileNotFoundException("Image file not found", imagePath);
            
            // Ensure qemu-img is available
            EnsureQemuImgAvailable();
            
            // Create a new file path for the converted image
            var outputPath = Path.Combine(
                Path.GetDirectoryName(imagePath),
                $"{Path.GetFileNameWithoutExtension(imagePath)}.{format}");
            
            try
            {
                progressAction?.Invoke($"Converting image to {format}...");
                
                // Run qemu-img convert command
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "qemu-img",
                        Arguments = $"convert -f {GetImageFormat(imagePath)} -O {format} \"{imagePath}\" \"{outputPath}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                
                process.Start();
                
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                
                process.WaitForExit();
                
                if (process.ExitCode != 0)
                {
                    throw new Exception($"Failed to convert image: {error}");
                }
                
                progressAction?.Invoke("Image converted successfully");
                
                return outputPath;
            }
            catch (Exception ex)
            {
                if (File.Exists(outputPath))
                    File.Delete(outputPath);
                
                throw new Exception($"Failed to convert image: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets the format of an image file
        /// </summary>
        /// <param name="imagePath">The path to the image file</param>
        /// <returns>The image format</returns>
        private static string GetImageFormat(string imagePath)
        {
            var extension = Path.GetExtension(imagePath).ToLowerInvariant();
            
            switch (extension)
            {
                case ".qcow2":
                    return "qcow2";
                case ".raw":
                case ".img":
                    return "raw";
                default:
                    // Try to detect the format using qemu-img
                    try
                    {
                        var process = new Process
                        {
                            StartInfo = new ProcessStartInfo
                            {
                                FileName = "qemu-img",
                                Arguments = $"info \"{imagePath}\"",
                                RedirectStandardOutput = true,
                                UseShellExecute = false,
                                CreateNoWindow = true
                            }
                        };
                        
                        process.Start();
                        var output = process.StandardOutput.ReadToEnd();
                        process.WaitForExit();
                        
                        if (output.Contains("file format: qcow2"))
                            return "qcow2";
                        else if (output.Contains("file format: raw"))
                            return "raw";
                        else
                            throw new Exception("Unknown image format");
                    }
                    catch
                    {
                        // Default to raw if detection fails
                        return "raw";
                    }
            }
        }

        /// <summary>
        /// Ensures that qemu-img is available
        /// </summary>
        private static void EnsureQemuImgAvailable()
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "qemu-img",
                        Arguments = "--version",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };
                
                process.Start();
                process.WaitForExit();
                
                if (process.ExitCode != 0)
                {
                    throw new Exception("qemu-img is not available");
                }
            }
            catch
            {
                throw new Exception("qemu-img is not available. Please install QEMU tools.");
            }
        }
    }
}
