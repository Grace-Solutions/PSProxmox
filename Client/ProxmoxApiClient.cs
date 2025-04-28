using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Management.Automation;
using PSProxmox.Session;
using PSProxmox.Utilities;

namespace PSProxmox.Client
{
    /// <summary>
    /// Client for making API requests to Proxmox VE.
    /// </summary>
    public class ProxmoxApiClient
    {
        private readonly ProxmoxConnection _connection;
        private readonly PSCmdlet _cmdlet;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxmoxApiClient"/> class.
        /// </summary>
        /// <param name="connection">The Proxmox connection.</param>
        /// <param name="cmdlet">The PowerShell cmdlet making the request.</param>
        public ProxmoxApiClient(ProxmoxConnection connection, PSCmdlet cmdlet)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _cmdlet = cmdlet;
        }

        /// <summary>
        /// Makes a GET request to the Proxmox API.
        /// </summary>
        /// <param name="endpoint">The API endpoint.</param>
        /// <returns>The response as a string.</returns>
        public string Get(string endpoint)
        {
            return SendRequest(endpoint, "GET", null);
        }

        /// <summary>
        /// Makes a POST request to the Proxmox API.
        /// </summary>
        /// <param name="endpoint">The API endpoint.</param>
        /// <param name="parameters">The request parameters.</param>
        /// <returns>The response as a string.</returns>
        public string Post(string endpoint, Dictionary<string, string> parameters)
        {
            return SendRequest(endpoint, "POST", parameters);
        }

        /// <summary>
        /// Makes a PUT request to the Proxmox API.
        /// </summary>
        /// <param name="endpoint">The API endpoint.</param>
        /// <param name="parameters">The request parameters.</param>
        /// <returns>The response as a string.</returns>
        public string Put(string endpoint, Dictionary<string, string> parameters)
        {
            return SendRequest(endpoint, "PUT", parameters);
        }

        /// <summary>
        /// Makes a DELETE request to the Proxmox API.
        /// </summary>
        /// <param name="endpoint">The API endpoint.</param>
        /// <returns>The response as a string.</returns>
        public string Delete(string endpoint)
        {
            return SendRequest(endpoint, "DELETE", null);
        }

        private string SendRequest(string endpoint, string method, Dictionary<string, string> parameters)
        {
            if (!_connection.IsAuthenticated)
            {
                throw new InvalidOperationException("Not authenticated. Call Connect-ProxmoxServer first.");
            }

            string url = $"{_connection.ApiUrl}/{endpoint}";
            
            // Log the URL if verbose is enabled
            if (_cmdlet != null)
            {
                _cmdlet.WriteVerbose($"Sending {method} request to {url}");
            }

            // Create the request
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method;
            request.Headers.Add("Cookie", $"PVEAuthCookie={_connection.Ticket}");
            
            if (method != "GET")
            {
                request.Headers.Add("CSRFPreventionToken", _connection.CSRFPreventionToken);
            }

            // Skip certificate validation if requested
            if (_connection.SkipCertificateValidation && request.RequestUri.Scheme == "https")
            {
                request.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            }

            // Add parameters for POST/PUT requests
            if (parameters != null && (method == "POST" || method == "PUT"))
            {
                string postData = string.Join("&", Array.ConvertAll(
                    parameters.Keys.ToArray(),
                    key => $"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(parameters[key])}"
                ));

                byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;

                using (Stream dataStream = request.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
            }

            // Get the response
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
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
                        throw new Exception($"API request failed: {errorText}");
                    }
                }
                throw;
            }
        }
    }
}
