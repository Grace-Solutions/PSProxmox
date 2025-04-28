using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security;
using System.Text;
using System.Management.Automation;
using Newtonsoft.Json;
using PSProxmox.Models;

namespace PSProxmox.Session
{
    /// <summary>
    /// Handles authentication and session management for Proxmox VE.
    /// </summary>
    public class ProxmoxSession
    {
        /// <summary>
        /// Authenticates with the Proxmox VE API.
        /// </summary>
        /// <param name="server">The server hostname or IP address.</param>
        /// <param name="port">The port number.</param>
        /// <param name="useSSL">Whether to use HTTPS.</param>
        /// <param name="skipCertificateValidation">Whether to skip SSL certificate validation.</param>
        /// <param name="username">The username for authentication.</param>
        /// <param name="password">The password for authentication.</param>
        /// <param name="realm">The realm for authentication.</param>
        /// <param name="cmdlet">The PowerShell cmdlet making the request.</param>
        /// <returns>A connection object with authentication information.</returns>
        public static ProxmoxConnection Login(
            string server,
            int port,
            bool useSSL,
            bool skipCertificateValidation,
            string username,
            SecureString password,
            string realm,
            PSCmdlet cmdlet)
        {
            var connection = new ProxmoxConnection(server, port, useSSL, skipCertificateValidation, username, realm);
            
            // Create the login URL
            string loginUrl = $"{connection.ApiUrl}/access/ticket";
            
            // Log the URL if verbose is enabled
            if (cmdlet != null)
            {
                cmdlet.WriteVerbose($"Authenticating to {loginUrl}");
            }

            // Create the request
            var request = (HttpWebRequest)WebRequest.Create(loginUrl);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            // Skip certificate validation if requested
            if (skipCertificateValidation && request.RequestUri.Scheme == "https")
            {
                request.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            }

            // Convert SecureString to plain text (only for sending to API)
            string plainPassword = new System.Net.NetworkCredential(string.Empty, password).Password;

            // Create the request body
            string postData = $"username={Uri.EscapeDataString(username)}&password={Uri.EscapeDataString(plainPassword)}&realm={Uri.EscapeDataString(realm)}";
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            request.ContentLength = byteArray.Length;

            // Send the request
            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            // Get the response
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string responseText = reader.ReadToEnd();
                    var loginResponse = JsonConvert.DeserializeObject<ProxmoxResponse<ProxmoxTicket>>(responseText);

                    if (loginResponse?.Data == null)
                    {
                        throw new Exception("Invalid response from server");
                    }

                    return connection.WithAuthentication(
                        loginResponse.Data.Ticket,
                        loginResponse.Data.CSRFPreventionToken);
                }
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    using (var errorResponse = (HttpWebResponse)ex.Response)
                    using (var reader = new StreamReader(errorResponse.GetResponseStream()))
                    {
                        string errorText = reader.ReadToEnd();
                        throw new Exception($"Authentication failed: {errorText}");
                    }
                }
                throw;
            }
        }

        /// <summary>
        /// Logs out from the Proxmox VE API.
        /// </summary>
        /// <param name="connection">The connection to log out from.</param>
        /// <param name="cmdlet">The PowerShell cmdlet making the request.</param>
        public static void Logout(ProxmoxConnection connection, PSCmdlet cmdlet)
        {
            if (connection == null || !connection.IsAuthenticated)
            {
                return;
            }

            // Create the logout URL
            string logoutUrl = $"{connection.ApiUrl}/access/ticket";
            
            // Log the URL if verbose is enabled
            if (cmdlet != null)
            {
                cmdlet.WriteVerbose($"Logging out from {logoutUrl}");
            }

            // Create the request
            var request = (HttpWebRequest)WebRequest.Create(logoutUrl);
            request.Method = "DELETE";
            request.Headers.Add("Cookie", $"PVEAuthCookie={connection.Ticket}");
            request.Headers.Add("CSRFPreventionToken", connection.CSRFPreventionToken);

            // Skip certificate validation if requested
            if (connection.SkipCertificateValidation && request.RequestUri.Scheme == "https")
            {
                request.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            }

            // Send the request and ignore the response
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    // Just to ensure the request is completed
                }
            }
            catch (WebException)
            {
                // Ignore errors during logout
            }
        }
    }
}
