using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <para type="synopsis">Gets virtual machines from a Proxmox VE server.</para>
    /// <para type="description">The Get-ProxmoxVM cmdlet retrieves virtual machines from a Proxmox VE server.</para>
    /// <example>
    ///   <para>Get all virtual machines</para>
    ///   <code>$vms = Get-ProxmoxVM -Connection $connection</code>
    /// </example>
    /// <example>
    ///   <para>Get a specific virtual machine by ID</para>
    ///   <code>$vm = Get-ProxmoxVM -Connection $connection -VMID 100</code>
    /// </example>
    /// <example>
    ///   <para>Get virtual machines on a specific node</para>
    ///   <code>$vms = Get-ProxmoxVM -Connection $connection -Node "pve1"</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "ProxmoxVM")]
    [OutputType(typeof(ProxmoxVM), typeof(string))]
    public class GetProxmoxVMCmdlet : FilteringCmdlet
    {

        /// <summary>
        /// <para type="description">The ID of the virtual machine to retrieve.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int? VMID { get; set; }

        /// <summary>
        /// <para type="description">The node to retrieve virtual machines from.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Node { get; set; }

        /// <summary>
        /// <para type="description">The name of the virtual machine to retrieve. Supports wildcards and regex when used with -UseRegex.</para>
        /// </summary>
        [Parameter(Mandatory = false, Position = 2)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">Whether to return the raw JSON response.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter RawJson { get; set; }

        /// <summary>
        /// Fetches guest agent information for a VM.
        /// </summary>
        /// <param name="client">The Proxmox API client.</param>
        /// <param name="node">The node name.</param>
        /// <param name="vmid">The VM ID.</param>
        /// <returns>Guest agent information.</returns>
        private ProxmoxVMGuestAgent FetchGuestAgentInfo(ProxmoxApiClient client, string node, int vmid)
        {
            var guestAgent = new ProxmoxVMGuestAgent();

            try
            {
                // First, check if guest agent is enabled in VM configuration
                string configResponse = client.Get($"nodes/{node}/qemu/{vmid}/config");
                var configData = JsonUtility.DeserializeResponse<JObject>(configResponse);

                if (configData != null && configData.ContainsKey("agent"))
                {
                    var agentConfig = configData["agent"].ToString();
                    guestAgent.IsEnabled = agentConfig == "1" || agentConfig.Contains("enabled=1");
                }

                // Try to get guest agent info to check if it's running
                try
                {
                    var infoParameters = new Dictionary<string, string>
                    {
                        ["command"] = "info"
                    };

                    string infoResponse = client.Post($"nodes/{node}/qemu/{vmid}/agent", infoParameters);
                    var infoData = JsonUtility.DeserializeResponse<JObject>(infoResponse);

                    if (infoData != null && infoData["result"] != null)
                    {
                        guestAgent.IsRunning = true;
                        guestAgent.IsAvailable = true;

                        var result = infoData["result"];
                        if (result["version"] != null)
                        {
                            guestAgent.Version = result["version"].ToString();
                        }
                    }
                }
                catch (Exception infoEx)
                {
                    guestAgent.IsRunning = false;
                    guestAgent.LastError = infoEx.Message;
                    WriteVerbose($"Guest agent info command failed for VM {vmid}: {infoEx.Message}");
                }

                // Try to get network interfaces from guest agent if it's running
                if (guestAgent.IsRunning)
                {
                    try
                    {
                        var parameters = new Dictionary<string, string>
                        {
                            ["command"] = "network-get-interfaces"
                        };

                        string response = client.Post($"nodes/{node}/qemu/{vmid}/agent", parameters);
                        var responseData = JsonUtility.DeserializeResponse<JObject>(response);

                        if (responseData != null && responseData["result"] != null)
                        {
                            var interfaces = responseData["result"].ToObject<List<ProxmoxVMGuestAgentNetworkInterface>>();
                            guestAgent.NetworkInterfaces = interfaces ?? new List<ProxmoxVMGuestAgentNetworkInterface>();

                            // Extract IP addresses
                            foreach (var iface in guestAgent.NetworkInterfaces)
                            {
                                if (iface.IPAddresses != null)
                                {
                                    foreach (var ip in iface.IPAddresses)
                                    {
                                        if (ip.Type == "ipv4" && !string.IsNullOrEmpty(ip.Address))
                                        {
                                            guestAgent.IPAddresses.IPv4.Add(ip.Address);
                                        }
                                        else if (ip.Type == "ipv6" && !string.IsNullOrEmpty(ip.Address))
                                        {
                                            guestAgent.IPAddresses.IPv6.Add(ip.Address);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception netEx)
                    {
                        guestAgent.LastError = netEx.Message;
                        WriteVerbose($"Guest agent network-get-interfaces failed for VM {vmid}: {netEx.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Guest agent not available or command failed
                WriteVerbose($"Guest agent configuration check failed for VM {vmid}: {ex.Message}");
                guestAgent.IsAvailable = false;
                guestAgent.LastError = ex.Message;
            }

            return guestAgent;
        }

        /// <summary>
        /// Populates the NetIf property for a VM by fetching network interface information.
        /// </summary>
        /// <param name="client">The Proxmox API client.</param>
        /// <param name="node">The node name.</param>
        /// <param name="vmid">The VM ID.</param>
        /// <param name="vm">The VM object to populate.</param>
        private void PopulateNetIfInfo(ProxmoxApiClient client, string node, int vmid, ProxmoxVM vm)
        {
            try
            {
                // Get VM configuration to extract network interface information
                string configResponse = client.Get($"nodes/{node}/qemu/{vmid}/config");
                var configData = JsonUtility.DeserializeResponse<JObject>(configResponse);

                if (configData != null)
                {
                    var netifList = new List<string>();

                    // Look for network interfaces (net0, net1, etc.)
                    foreach (var property in configData.Properties())
                    {
                        if (property.Name.StartsWith("net") && char.IsDigit(property.Name.Last()))
                        {
                            netifList.Add($"{property.Name}={property.Value}");
                        }
                    }

                    // Set the NetIf property
                    vm.NetIf = string.Join(",", netifList);
                }
            }
            catch (Exception ex)
            {
                WriteVerbose($"Failed to populate NetIf for VM {vmid}: {ex.Message}");
                // Don't fail the entire operation, just leave NetIf empty
            }
        }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var connection = GetConnection();
                ValidateConnection(connection);
                var client = new ProxmoxApiClient(connection, this);
                string response;

                if (VMID.HasValue)
                {
                    // Get a specific VM
                    if (string.IsNullOrEmpty(Node))
                    {
                        // First, get the list of nodes
                        string nodesResponse = client.Get("nodes");
                        var nodesData = JsonUtility.DeserializeResponse<JArray>(nodesResponse);

                        // Search for the VM on each node
                        foreach (var nodeObj in nodesData)
                        {
                            string nodeName = nodeObj["node"].ToString();
                            try
                            {
                                response = client.Get($"nodes/{nodeName}/qemu/{VMID.Value}/status/current");
                                var vm = JsonUtility.DeserializeResponse<ProxmoxVM>(response);
                                vm.Node = nodeName;
                                vm.VMID = VMID.Value;

                                // Populate NetIf information
                                PopulateNetIfInfo(client, nodeName, VMID.Value, vm);

                                // Fetch guest agent information
                                vm.GuestAgent = FetchGuestAgentInfo(client, nodeName, VMID.Value);

                                if (RawJson.IsPresent)
                                {
                                    WriteObject(response);
                                }
                                else
                                {
                                    WriteObject(vm);
                                }
                                return;
                            }
                            catch
                            {
                                // VM not found on this node, continue to the next one
                            }
                        }

                        WriteError(new ErrorRecord(
                            new Exception($"VM with ID {VMID.Value} not found on any node"),
                            "VMNotFound",
                            ErrorCategory.ObjectNotFound,
                            VMID.Value));
                    }
                    else
                    {
                        // Get the VM from the specified node
                        response = client.Get($"nodes/{Node}/qemu/{VMID.Value}/status/current");
                        var vm = JsonUtility.DeserializeResponse<ProxmoxVM>(response);
                        vm.Node = Node;
                        vm.VMID = VMID.Value;

                        // Populate NetIf information
                        PopulateNetIfInfo(client, Node, VMID.Value, vm);

                        // Fetch guest agent information
                        vm.GuestAgent = FetchGuestAgentInfo(client, Node, VMID.Value);

                        if (RawJson.IsPresent)
                        {
                            WriteObject(response);
                        }
                        else
                        {
                            WriteObject(vm);
                        }
                    }
                }
                else
                {
                    // Get all VMs
                    if (string.IsNullOrEmpty(Node))
                    {
                        // First, get the list of nodes
                        string nodesResponse = client.Get("nodes");
                        var nodesData = JsonUtility.DeserializeResponse<JArray>(nodesResponse);
                        var allVMs = new List<ProxmoxVM>();

                        // Get VMs from each node
                        foreach (var nodeObj in nodesData)
                        {
                            string nodeName = nodeObj["node"].ToString();
                            try
                            {
                                response = client.Get($"nodes/{nodeName}/qemu");
                                var vms = JsonUtility.DeserializeResponse<JArray>(response);

                                foreach (var vmObj in vms)
                                {
                                    var vm = vmObj.ToObject<ProxmoxVM>();
                                    vm.Node = nodeName;

                                    // Populate NetIf information
                                    PopulateNetIfInfo(client, nodeName, vm.VMID, vm);

                                    // Fetch guest agent information
                                    vm.GuestAgent = FetchGuestAgentInfo(client, nodeName, vm.VMID);

                                    allVMs.Add(vm);
                                }
                            }
                            catch
                            {
                                // Error getting VMs from this node, continue to the next one
                                WriteWarning($"Failed to get VMs from node {nodeName}");
                            }
                        }

                        // Apply name filtering if specified
                        if (!string.IsNullOrEmpty(Name))
                        {
                            allVMs = FilterByProperty(allVMs, "Name", Name).ToList();
                        }

                        if (RawJson.IsPresent)
                        {
                            WriteObject(JsonConvert.SerializeObject(allVMs));
                        }
                        else
                        {
                            WriteObject(allVMs, true);
                        }
                    }
                    else
                    {
                        // Get VMs from the specified node
                        response = client.Get($"nodes/{Node}/qemu");
                        var vms = JsonUtility.DeserializeResponse<JArray>(response);
                        var nodeVMs = new List<ProxmoxVM>();

                        foreach (var vmObj in vms)
                        {
                            var vm = vmObj.ToObject<ProxmoxVM>();
                            vm.Node = Node;

                            // Populate NetIf information
                            PopulateNetIfInfo(client, Node, vm.VMID, vm);

                            // Fetch guest agent information
                            vm.GuestAgent = FetchGuestAgentInfo(client, Node, vm.VMID);

                            nodeVMs.Add(vm);
                        }

                        // Apply name filtering if specified
                        if (!string.IsNullOrEmpty(Name))
                        {
                            nodeVMs = FilterByProperty(nodeVMs, "Name", Name).ToList();
                        }

                        if (RawJson.IsPresent)
                        {
                            WriteObject(response);
                        }
                        else
                        {
                            WriteObject(nodeVMs, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetProxmoxVMError", ErrorCategory.OperationStopped, null));
            }
        }
    }
}
