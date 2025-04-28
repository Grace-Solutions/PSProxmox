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
    /// <para type="synopsis">Gets SDN zones from Proxmox VE.</para>
    /// <para type="description">The Get-ProxmoxSDNZone cmdlet retrieves SDN zones from Proxmox VE.</para>
    /// <example>
    ///   <para>Get all SDN zones</para>
    ///   <code>$zones = Get-ProxmoxSDNZone -Connection $connection</code>
    /// </example>
    /// <example>
    ///   <para>Get a specific SDN zone by name</para>
    ///   <code>$zone = Get-ProxmoxSDNZone -Connection $connection -Zone "zone1"</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "ProxmoxSDNZone")]
    [OutputType(typeof(ProxmoxSDNZone), typeof(string))]
    public class GetProxmoxSDNZoneCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The name of the zone to retrieve.</para>
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

                if (string.IsNullOrEmpty(Zone))
                {
                    // Get all zones
                    response = client.Get("sdn/zones");
                    var zonesData = JsonUtility.DeserializeResponse<JArray>(response);
                    var zones = new List<ProxmoxSDNZone>();

                    foreach (var zoneObj in zonesData)
                    {
                        var zone = zoneObj.ToObject<ProxmoxSDNZone>();
                        zones.Add(zone);
                    }

                    if (RawJson.IsPresent)
                    {
                        WriteObject(response);
                    }
                    else
                    {
                        WriteObject(zones, true);
                    }
                }
                else
                {
                    // Get a specific zone
                    response = client.Get($"sdn/zones/{Zone}");
                    var zoneData = JsonUtility.DeserializeResponse<JObject>(response);
                    var zone = zoneData.ToObject<ProxmoxSDNZone>();

                    if (RawJson.IsPresent)
                    {
                        WriteObject(response);
                    }
                    else
                    {
                        WriteObject(zone);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetProxmoxSDNZoneError", ErrorCategory.OperationStopped, Connection));
            }
        }
    }
}
