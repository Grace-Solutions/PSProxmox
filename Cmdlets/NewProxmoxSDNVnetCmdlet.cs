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
    /// <para type="synopsis">Creates a new SDN VNet in Proxmox VE.</para>
    /// <para type="description">The New-ProxmoxSDNVnet cmdlet creates a new SDN VNet in Proxmox VE.</para>
    /// <example>
    ///   <para>Create a new SDN VNet</para>
    ///   <code>$vnet = New-ProxmoxSDNVnet -Connection $connection -VNet "vnet1" -Zone "zone1" -IPv4 "192.168.1.0/24" -Gateway "192.168.1.1"</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "ProxmoxSDNVnet")]
    [OutputType(typeof(ProxmoxSDNVnet))]
    public class NewProxmoxSDNVnetCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The name of the VNet.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public string VNet { get; set; }

        /// <summary>
        /// <para type="description">The zone of the VNet.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public string Zone { get; set; }

        /// <summary>
        /// <para type="description">The alias of the VNet.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Alias { get; set; }

        /// <summary>
        /// <para type="description">The VLAN ID of the VNet.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int? VlanID { get; set; }

        /// <summary>
        /// <para type="description">The tag of the VNet.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int? Tag { get; set; }

        /// <summary>
        /// <para type="description">The IPv4 subnet of the VNet.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string IPv4 { get; set; }

        /// <summary>
        /// <para type="description">The IPv6 subnet of the VNet.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string IPv6 { get; set; }

        /// <summary>
        /// <para type="description">The MAC address of the VNet.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string MAC { get; set; }

        /// <summary>
        /// <para type="description">The gateway of the VNet.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Gateway { get; set; }

        /// <summary>
        /// <para type="description">The IPv6 gateway of the VNet.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Gateway6 { get; set; }

        /// <summary>
        /// <para type="description">The MTU of the VNet.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int? MTU { get; set; }

        /// <summary>
        /// <para type="description">Whether DHCP is enabled for the VNet.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter DHCP { get; set; }

        /// <summary>
        /// <para type="description">The DNS servers of the VNet.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string DNS { get; set; }

        /// <summary>
        /// <para type="description">The DNS search domain of the VNet.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string DNSSearchDomain { get; set; }

        /// <summary>
        /// <para type="description">The reverse DNS of the VNet.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string ReverseDNS { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);

                // Create the VNet
                var parameters = new Dictionary<string, string>
                {
                    ["vnet"] = VNet,
                    ["zone"] = Zone
                };

                if (!string.IsNullOrEmpty(Alias))
                {
                    parameters["alias"] = Alias;
                }

                if (VlanID.HasValue)
                {
                    parameters["vlanid"] = VlanID.Value.ToString();
                }

                if (Tag.HasValue)
                {
                    parameters["tag"] = Tag.Value.ToString();
                }

                if (!string.IsNullOrEmpty(IPv4))
                {
                    parameters["ipv4"] = IPv4;
                }

                if (!string.IsNullOrEmpty(IPv6))
                {
                    parameters["ipv6"] = IPv6;
                }

                if (!string.IsNullOrEmpty(MAC))
                {
                    parameters["mac"] = MAC;
                }

                if (!string.IsNullOrEmpty(Gateway))
                {
                    parameters["gateway"] = Gateway;
                }

                if (!string.IsNullOrEmpty(Gateway6))
                {
                    parameters["gateway6"] = Gateway6;
                }

                if (MTU.HasValue)
                {
                    parameters["mtu"] = MTU.Value.ToString();
                }

                parameters["dhcp"] = DHCP.IsPresent ? "1" : "0";

                if (!string.IsNullOrEmpty(DNS))
                {
                    parameters["dns"] = DNS;
                }

                if (!string.IsNullOrEmpty(DNSSearchDomain))
                {
                    parameters["dnssearchdomain"] = DNSSearchDomain;
                }

                if (!string.IsNullOrEmpty(ReverseDNS))
                {
                    parameters["reversedns"] = ReverseDNS;
                }

                // Create the VNet
                client.Post("sdn/vnets", parameters);

                // Get the created VNet
                string vnetResponse = client.Get($"sdn/vnets/{VNet}");
                var vnet = JsonUtility.DeserializeResponse<ProxmoxSDNVnet>(vnetResponse);

                WriteObject(vnet);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "NewProxmoxSDNVnetError", ErrorCategory.OperationStopped, Connection));
            }
        }
    }
}
