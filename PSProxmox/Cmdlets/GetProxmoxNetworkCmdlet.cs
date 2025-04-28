using System;
using System.Collections.Generic;
using System.Management.Automation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PSProxmox.Client;
using PSProxmox.Models;
using PSProxmox.Session;
using PSProxmox.Utilities;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Gets network interfaces from Proxmox VE.</para>
    /// <para type="description">The Get-ProxmoxNetwork cmdlet retrieves network interfaces from Proxmox VE.</para>
    /// <example>
    ///   <para>Get all network interfaces</para>
    ///   <code>$networks = Get-ProxmoxNetwork -Connection $connection -Node "pve1"</code>
    /// </example>
    /// <example>
    ///   <para>Get a specific network interface by name</para>
    ///   <code>$network = Get-ProxmoxNetwork -Connection $connection -Node "pve1" -Interface "vmbr0"</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "ProxmoxNetwork")]
    [OutputType(typeof(ProxmoxNetwork), typeof(string))]
    public class GetProxmoxNetworkCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The node to retrieve network interfaces from.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public string Node { get; set; }

        /// <summary>
        /// <para type="description">The name of the interface to retrieve.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Interface { get; set; }

        /// <summary>
        /// <para type="description">Whether to return the raw JSON response.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter RawJson { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);
                string response;

                if (string.IsNullOrEmpty(Interface))
                {
                    // Get all network interfaces
                    response = client.Get($"nodes/{Node}/network");
                    var networksData = JsonUtility.DeserializeResponse<JArray>(response);
                    var networks = new List<ProxmoxNetwork>();

                    foreach (var networkObj in networksData)
                    {
                        var network = networkObj.ToObject<ProxmoxNetwork>();
                        network.Node = Node;
                        networks.Add(network);
                    }

                    if (RawJson.IsPresent)
                    {
                        WriteObject(response);
                    }
                    else
                    {
                        WriteObject(networks, true);
                    }
                }
                else
                {
                    // Get a specific network interface
                    response = client.Get($"nodes/{Node}/network/{Interface}");
                    var networkData = JsonUtility.DeserializeResponse<JObject>(response);
                    var network = networkData.ToObject<ProxmoxNetwork>();
                    network.Node = Node;

                    if (RawJson.IsPresent)
                    {
                        WriteObject(response);
                    }
                    else
                    {
                        WriteObject(network);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetProxmoxNetworkError", ErrorCategory.OperationStopped, Connection));
            }
        }
    }
}
