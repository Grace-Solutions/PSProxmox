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
    /// <para type="synopsis">Gets SDN VNets from Proxmox VE.</para>
    /// <para type="description">The Get-ProxmoxSDNVnet cmdlet retrieves SDN VNets from Proxmox VE.</para>
    /// <example>
    ///   <para>Get all SDN VNets</para>
    ///   <code>$vnets = Get-ProxmoxSDNVnet -Connection $connection</code>
    /// </example>
    /// <example>
    ///   <para>Get a specific SDN VNet by name</para>
    ///   <code>$vnet = Get-ProxmoxSDNVnet -Connection $connection -VNet "vnet1"</code>
    /// </example>
    /// <example>
    ///   <para>Get all SDN VNets in a specific zone</para>
    ///   <code>$vnets = Get-ProxmoxSDNVnet -Connection $connection -Zone "zone1"</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "ProxmoxSDNVnet")]
    [OutputType(typeof(ProxmoxSDNVnet), typeof(string))]
    public class GetProxmoxSDNVnetCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The name of the VNet to retrieve.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string VNet { get; set; }

        /// <summary>
        /// <para type="description">The name of the zone to retrieve VNets from.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Zone { get; set; }

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

                if (string.IsNullOrEmpty(VNet))
                {
                    // Get all VNets or VNets in a specific zone
                    string endpoint = string.IsNullOrEmpty(Zone) ? "sdn/vnets" : $"sdn/zones/{Zone}/vnets";
                    response = client.Get(endpoint);
                    var vnetsData = JsonUtility.DeserializeResponse<JArray>(response);
                    var vnets = new List<ProxmoxSDNVnet>();

                    foreach (var vnetObj in vnetsData)
                    {
                        var vnet = vnetObj.ToObject<ProxmoxSDNVnet>();
                        vnets.Add(vnet);
                    }

                    if (RawJson.IsPresent)
                    {
                        WriteObject(response);
                    }
                    else
                    {
                        WriteObject(vnets, true);
                    }
                }
                else
                {
                    // Get a specific VNet
                    string endpoint = string.IsNullOrEmpty(Zone) ? $"sdn/vnets/{VNet}" : $"sdn/zones/{Zone}/vnets/{VNet}";
                    response = client.Get(endpoint);
                    var vnetData = JsonUtility.DeserializeResponse<JObject>(response);
                    var vnet = vnetData.ToObject<ProxmoxSDNVnet>();

                    if (RawJson.IsPresent)
                    {
                        WriteObject(response);
                    }
                    else
                    {
                        WriteObject(vnet);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetProxmoxSDNVnetError", ErrorCategory.OperationStopped, Connection));
            }
        }
    }
}
