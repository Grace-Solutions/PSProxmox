using System;
using System.Collections.Generic;
using System.Management.Automation;
using PSProxmox.Client;
using PSProxmox.Models;
using PSProxmox.Session;
using PSProxmox.Utilities;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Creates a new SDN zone in Proxmox VE.</para>
    /// <para type="description">The New-ProxmoxSDNZone cmdlet creates a new SDN zone in Proxmox VE.</para>
    /// <example>
    ///   <para>Create a new SDN zone</para>
    ///   <code>$zone = New-ProxmoxSDNZone -Connection $connection -Zone "zone1" -Type "vlan" -Bridge "vmbr0"</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "ProxmoxSDNZone")]
    [OutputType(typeof(ProxmoxSDNZone))]
    public class NewProxmoxSDNZoneCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The name of the zone.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public string Zone { get; set; }

        /// <summary>
        /// <para type="description">The type of the zone.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateSet("vlan", "vxlan", "qinq", "simple")]
        public string Type { get; set; }

        /// <summary>
        /// <para type="description">The bridge of the zone.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Bridge { get; set; }

        /// <summary>
        /// <para type="description">The DNS servers of the zone.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string DNS { get; set; }

        /// <summary>
        /// <para type="description">The DNS search domains of the zone.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string DNSZone { get; set; }

        /// <summary>
        /// <para type="description">The DHCP server of the zone.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string DHCP { get; set; }

        /// <summary>
        /// <para type="description">The reverse DNS of the zone.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string ReverseDNS { get; set; }

        /// <summary>
        /// <para type="description">The IPv6 of the zone.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string IPv6 { get; set; }

        /// <summary>
        /// <para type="description">The MTU of the zone.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int? MTU { get; set; }

        /// <summary>
        /// <para type="description">Whether the zone is VLAN aware.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter VLANAware { get; set; }

        /// <summary>
        /// <para type="description">The controller of the zone.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Controller { get; set; }

        /// <summary>
        /// <para type="description">The gateway of the zone.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Gateway { get; set; }

        /// <summary>
        /// <para type="description">The MAC prefix of the zone.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string MACPrefix { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);

                // Create the zone
                var parameters = new Dictionary<string, string>
                {
                    ["zone"] = Zone,
                    ["type"] = Type
                };

                if (!string.IsNullOrEmpty(Bridge))
                {
                    parameters["bridge"] = Bridge;
                }

                if (!string.IsNullOrEmpty(DNS))
                {
                    parameters["dns"] = DNS;
                }

                if (!string.IsNullOrEmpty(DNSZone))
                {
                    parameters["dnszone"] = DNSZone;
                }

                if (!string.IsNullOrEmpty(DHCP))
                {
                    parameters["dhcp"] = DHCP;
                }

                if (!string.IsNullOrEmpty(ReverseDNS))
                {
                    parameters["reversedns"] = ReverseDNS;
                }

                if (!string.IsNullOrEmpty(IPv6))
                {
                    parameters["ipv6"] = IPv6;
                }

                if (MTU.HasValue)
                {
                    parameters["mtu"] = MTU.Value.ToString();
                }

                parameters["vlanaware"] = VLANAware.IsPresent ? "1" : "0";

                if (!string.IsNullOrEmpty(Controller))
                {
                    parameters["controller"] = Controller;
                }

                if (!string.IsNullOrEmpty(Gateway))
                {
                    parameters["gateway"] = Gateway;
                }

                if (!string.IsNullOrEmpty(MACPrefix))
                {
                    parameters["mac_prefix"] = MACPrefix;
                }

                // Create the zone
                client.Post("sdn/zones", parameters);

                // Get the created zone
                string zoneResponse = client.Get($"sdn/zones/{Zone}");
                var zone = JsonUtility.DeserializeResponse<ProxmoxSDNZone>(zoneResponse);

                WriteObject(zone);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "NewProxmoxSDNZoneError", ErrorCategory.OperationStopped, Connection));
            }
        }
    }
}
