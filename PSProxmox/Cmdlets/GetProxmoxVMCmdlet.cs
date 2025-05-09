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
