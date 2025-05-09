using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Management.Automation;

namespace PSProxmox.CloudImages
{
    /// <summary>
    /// Provides functionality for downloading cloud images
    /// </summary>
    public class CloudImageDownloader
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private static readonly string _downloadDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "PSProxmox", "CloudImageDownloads");

        /// <summary>
        /// Downloads a cloud image with progress reporting
        /// </summary>
        /// <param name="image">The cloud image to download</param>
        /// <param name="progressAction">Action to report progress</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The path to the downloaded image</returns>
        public static async Task<string> DownloadImageAsync(
            CloudImage image, 
            Action<long, long> progressAction, 
            CancellationToken cancellationToken = default)
        {
            EnsureDownloadDirectoryExists();
            
            var localPath = Path.Combine(_downloadDirectory, image.Filename);
            
            // Check if the file already exists
            if (File.Exists(localPath))
            {
                // Verify the file integrity
                if (await VerifyFileIntegrityAsync(localPath, image.Url))
                {
                    return localPath;
                }
                
                // If verification fails, delete the file and download again
                File.Delete(localPath);
            }
            
            // Download the file
            using (var response = await _httpClient.GetAsync(image.Url, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                
                var totalBytes = response.Content.Headers.ContentLength ?? -1L;
                var buffer = new byte[8192];
                var bytesRead = 0L;
                
                using (var fileStream = new FileStream(localPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                using (var contentStream = await response.Content.ReadAsStreamAsync())
                {
                    while (true)
                    {
                        var read = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken);
                        if (read == 0)
                            break;
                        
                        await fileStream.WriteAsync(buffer, 0, read, cancellationToken);
                        
                        bytesRead += read;
                        progressAction?.Invoke(bytesRead, totalBytes);
                        
                        if (cancellationToken.IsCancellationRequested)
                        {
                            // Clean up the partial download
                            fileStream.Close();
                            File.Delete(localPath);
                            cancellationToken.ThrowIfCancellationRequested();
                        }
                    }
                }
            }
            
            return localPath;
        }

        /// <summary>
        /// Verifies the integrity of a downloaded file by comparing its size with the remote file
        /// </summary>
        /// <param name="localPath">The path to the local file</param>
        /// <param name="url">The URL of the remote file</param>
        /// <returns>True if the file integrity is verified, false otherwise</returns>
        private static async Task<bool> VerifyFileIntegrityAsync(string localPath, string url)
        {
            try
            {
                var fileInfo = new FileInfo(localPath);
                
                // Get the remote file size
                using (var response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url)))
                {
                    if (!response.IsSuccessStatusCode)
                        return false;
                    
                    var remoteSize = response.Content.Headers.ContentLength;
                    if (!remoteSize.HasValue)
                        return false;
                    
                    // Compare the file sizes
                    return fileInfo.Length == remoteSize.Value;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Ensures the download directory exists
        /// </summary>
        private static void EnsureDownloadDirectoryExists()
        {
            if (!Directory.Exists(_downloadDirectory))
            {
                Directory.CreateDirectory(_downloadDirectory);
            }
        }
    }
}
