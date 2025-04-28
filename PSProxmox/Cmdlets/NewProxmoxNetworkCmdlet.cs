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
    /// <para type="synopsis">Creates a new network interface in Proxmox VE.</para>
    /// <para type="description">The New-ProxmoxNetwork cmdlet creates a new network interface in Proxmox VE.</para>
    /// <example>
    ///   <para>Create a new bridge interface</para>
    ///   <code>$network = New-ProxmoxNetwork -Connection $connection -Node "pve1" -Interface "vmbr1" -Type "bridge" -BridgePorts "eth1" -Method "static" -Address "192.168.2.1" -Netmask "255.255.255.0" -Autostart</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "ProxmoxNetwork")]
    [OutputType(typeof(ProxmoxNetwork))]
    public class NewProxmoxNetworkCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The node to create the network interface on.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public string Node { get; set; }

        /// <summary>
        /// <para type="description">The name of the interface.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public string Interface { get; set; }

        /// <summary>
        /// <para type="description">The type of the interface.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateSet("bridge", "bond", "eth", "vlan")]
        public string Type { get; set; }

        /// <summary>
        /// <para type="description">The method of the interface.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        [ValidateSet("static", "dhcp", "manual")]
        public string Method { get; set; } = "static";

        /// <summary>
        /// <para type="description">The IP address of the interface.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Address { get; set; }

        /// <summary>
        /// <para type="description">The netmask of the interface.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Netmask { get; set; }

        /// <summary>
        /// <para type="description">The gateway of the interface.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Gateway { get; set; }

        /// <summary>
        /// <para type="description">The bridge ports of the interface.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string BridgePorts { get; set; }

        /// <summary>
        /// <para type="description">The bridge STP of the interface.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter BridgeSTP { get; set; }

        /// <summary>
        /// <para type="description">The bridge FD of the interface.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int? BridgeFD { get; set; }

        /// <summary>
        /// <para type="description">The bond slaves of the interface.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string BondSlaves { get; set; }

        /// <summary>
        /// <para type="description">The bond mode of the interface.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        [ValidateSet("balance-rr", "active-backup", "balance-xor", "broadcast", "802.3ad", "balance-tlb", "balance-alb")]
        public string BondMode { get; set; }

        /// <summary>
        /// <para type="description">The VLAN ID of the interface.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int? VlanID { get; set; }

        /// <summary>
        /// <para type="description">The VLAN raw device of the interface.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string VlanRawDevice { get; set; }

        /// <summary>
        /// <para type="description">Whether the interface should autostart.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Autostart { get; set; }

        /// <summary>
        /// <para type="description">The comments of the interface.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Comments { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);

                // Create the network interface
                var parameters = new Dictionary<string, string>
                {
                    ["iface"] = Interface,
                    ["type"] = Type
                };

                if (!string.IsNullOrEmpty(Method))
                {
                    parameters["method"] = Method;
                }

                if (!string.IsNullOrEmpty(Address))
                {
                    parameters["address"] = Address;
                }

                if (!string.IsNullOrEmpty(Netmask))
                {
                    parameters["netmask"] = Netmask;
                }

                if (!string.IsNullOrEmpty(Gateway))
                {
                    parameters["gateway"] = Gateway;
                }

                if (!string.IsNullOrEmpty(BridgePorts))
                {
                    parameters["bridge_ports"] = BridgePorts;
                }

                if (BridgeSTP.IsPresent)
                {
                    parameters["bridge_stp"] = "on";
                }

                if (BridgeFD.HasValue)
                {
                    parameters["bridge_fd"] = BridgeFD.Value.ToString();
                }

                if (!string.IsNullOrEmpty(BondSlaves))
                {
                    parameters["bond_slaves"] = BondSlaves;
                }

                if (!string.IsNullOrEmpty(BondMode))
                {
                    parameters["bond_mode"] = BondMode;
                }

                if (VlanID.HasValue)
                {
                    parameters["vlan-id"] = VlanID.Value.ToString();
                }

                if (!string.IsNullOrEmpty(VlanRawDevice))
                {
                    parameters["vlan-raw-device"] = VlanRawDevice;
                }

                parameters["autostart"] = Autostart.IsPresent ? "1" : "0";

                if (!string.IsNullOrEmpty(Comments))
                {
                    parameters["comments"] = Comments;
                }

                // Create the network interface
                client.Post($"nodes/{Node}/network", parameters);

                // Apply the network configuration
                client.Put($"nodes/{Node}/network", new Dictionary<string, string>());

                // Get the created network interface
                string networkResponse = client.Get($"nodes/{Node}/network/{Interface}");
                var network = JsonUtility.DeserializeResponse<ProxmoxNetwork>(networkResponse);
                network.Node = Node;

                WriteObject(network);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "NewProxmoxNetworkError", ErrorCategory.OperationStopped, Connection));
            }
        }
    }
}
