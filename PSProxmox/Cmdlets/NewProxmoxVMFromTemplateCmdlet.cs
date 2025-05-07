using System;
using System.Collections.Generic;
using System.Management.Automation;
using PSProxmox.Client;
using PSProxmox.Models;
using PSProxmox.Session;
using PSProxmox.Templates;
using PSProxmox.Utilities;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Creates a new virtual machine from a template in Proxmox VE.</para>
    /// <para type="description">The New-ProxmoxVMFromTemplate cmdlet creates a new virtual machine from a template in Proxmox VE.</para>
    /// <example>
    ///   <para>Create a new VM from a template</para>
    ///   <code>$vm = New-ProxmoxVMFromTemplate -Connection $connection -Node "pve1" -TemplateName "Ubuntu-Template" -Name "web01" -Start</code>
    /// </example>
    /// <example>
    ///   <para>Create multiple VMs from a template with a prefix and counter</para>
    ///   <code>$vms = New-ProxmoxVMFromTemplate -Connection $connection -Node "pve1" -TemplateName "Ubuntu-Template" -Prefix "web" -Count 3 -Start</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "ProxmoxVMFromTemplate")]
    [OutputType(typeof(ProxmoxVM))]
    public class NewProxmoxVMFromTemplateCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The node to create the VM on.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public string Node { get; set; }

        /// <summary>
        /// <para type="description">The name of the template to use.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public string TemplateName { get; set; }

        /// <summary>
        /// <para type="description">The name of the VM to create.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "SingleVM")]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">The prefix for the VM names when creating multiple VMs.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "MultipleVMs")]
        public string Prefix { get; set; }

        /// <summary>
        /// <para type="description">The number of VMs to create.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "MultipleVMs")]
        public int Count { get; set; }

        /// <summary>
        /// <para type="description">The starting index for the VM names when creating multiple VMs.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "MultipleVMs")]
        public int StartIndex { get; set; } = 1;

        /// <summary>
        /// <para type="description">The number of digits to use for the counter in VM names (e.g., 3 would result in "Prefix-001").</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "MultipleVMs")]
        [ValidateRange(1, 10)]
        public int CounterDigits { get; set; } = 1;

        /// <summary>
        /// <para type="description">The VM ID. If not specified, the next available ID will be used.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "SingleVM")]
        public int? VMID { get; set; }

        /// <summary>
        /// <para type="description">Whether to start the VM after creation.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Start { get; set; }

        /// <summary>
        /// <para type="description">The IP pool to use for assigning an IP address.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string IPPool { get; set; }

        /// <summary>
        /// <para type="description">The amount of memory in MB. If specified, overrides the template value.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int? Memory { get; set; }

        /// <summary>
        /// <para type="description">The number of CPU cores. If specified, overrides the template value.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int? Cores { get; set; }

        /// <summary>
        /// <para type="description">The disk size in GB. If specified, overrides the template value.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int? DiskSize { get; set; }

        /// <summary>
        /// <para type="description">The storage location for the disk. If specified, overrides the template value.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Storage { get; set; }

        /// <summary>
        /// <para type="description">The network interface model. If specified, overrides the template value.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string NetworkModel { get; set; }

        /// <summary>
        /// <para type="description">The network bridge. If specified, overrides the template value.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string NetworkBridge { get; set; }

        /// <summary>
        /// <para type="description">The description of the VM. If specified, overrides the template value.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Description { get; set; }

        /// <summary>
        /// <para type="description">Whether to automatically generate SMBIOS values.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter AutomaticSMBIOS { get; set; }

        /// <summary>
        /// <para type="description">The manufacturer profile to use for SMBIOS values. Valid values are: Proxmox, Dell, HP, Lenovo, Microsoft, VMware, HyperV, VirtualBox, Random.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        [ValidateSet("Proxmox", "Dell", "HP", "Lenovo", "Microsoft", "VMware", "HyperV", "VirtualBox", "Random")]
        public string SMBIOSProfile { get; set; } = "Random";

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);

                // Get the template
                ProxmoxVMTemplate template;
                try
                {
                    template = TemplateManager.GetTemplate(TemplateName);
                }
                catch (Exception ex)
                {
                    WriteError(new ErrorRecord(
                        new Exception($"Template '{TemplateName}' not found: {ex.Message}"),
                        "TemplateNotFound",
                        ErrorCategory.ObjectNotFound,
                        TemplateName));
                    return;
                }

                // Create VMs
                if (ParameterSetName == "SingleVM")
                {
                    // Create a single VM
                    var vm = CreateVMFromTemplate(client, template, Name, VMID);
                    WriteObject(vm);
                }
                else
                {
                    // Create multiple VMs
                    var vms = new List<ProxmoxVM>();
                    for (int i = 0; i < Count; i++)
                    {
                        // Format the counter with the specified number of digits
                        string counterFormat = new string('0', CounterDigits);
                        string formattedCounter = (StartIndex + i).ToString(counterFormat);
                        string vmName = $"{Prefix}{formattedCounter}";

                        var vm = CreateVMFromTemplate(client, template, vmName, null);
                        vms.Add(vm);
                    }
                    WriteObject(vms, true);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "NewProxmoxVMFromTemplateError", ErrorCategory.OperationStopped, Connection));
            }
        }

        private ProxmoxVM CreateVMFromTemplate(ProxmoxApiClient client, ProxmoxVMTemplate template, string vmName, int? vmId)
        {
            // Get the next available VMID if not specified
            if (!vmId.HasValue)
            {
                string response = client.Get("cluster/nextid");
                var nextId = JsonUtility.DeserializeResponse<string>(response);
                vmId = int.Parse(nextId);
            }

            // Clone the template VM
            var parameters = new Dictionary<string, string>
            {
                ["newid"] = vmId.Value.ToString(),
                ["name"] = vmName
            };

            if (Memory.HasValue)
            {
                parameters["memory"] = Memory.Value.ToString();
            }

            if (Cores.HasValue)
            {
                parameters["cores"] = Cores.Value.ToString();
            }

            if (DiskSize.HasValue)
            {
                parameters["disksize"] = DiskSize.Value.ToString();
            }

            if (!string.IsNullOrEmpty(Storage))
            {
                parameters["storage"] = Storage;
            }

            if (!string.IsNullOrEmpty(Description))
            {
                parameters["description"] = Description;
            }

            // Clone the VM
            WriteVerbose($"Creating VM {vmName} from template {template.Name}");
            client.Post($"nodes/{template.Node}/qemu/{template.VMID}/clone", parameters);

            // Get the created VM
            string vmResponse = client.Get($"nodes/{Node}/qemu/{vmId.Value}/status/current");
            var vm = JsonUtility.DeserializeResponse<ProxmoxVM>(vmResponse);
            vm.Node = Node;
            vm.VMID = vmId.Value;

            // Update network settings if specified
            if (!string.IsNullOrEmpty(NetworkModel) || !string.IsNullOrEmpty(NetworkBridge) || AutomaticSMBIOS.IsPresent)
            {
                var configParams = new Dictionary<string, string>();

                // Add network settings if specified
                if (!string.IsNullOrEmpty(NetworkModel) || !string.IsNullOrEmpty(NetworkBridge))
                {
                    string netModel = NetworkModel ?? "virtio";
                    string netBridge = NetworkBridge ?? "vmbr0";
                    configParams["net0"] = $"{netModel},bridge={netBridge}";
                }

                // Add SMBIOS settings if requested
                if (AutomaticSMBIOS.IsPresent)
                {
                    var smbios = Models.ProxmoxVMSMBIOSProfile.GetProfile(SMBIOSProfile);
                    string smbiosString = smbios.ToProxmoxString();
                    if (!string.IsNullOrEmpty(smbiosString))
                    {
                        configParams["smbios"] = smbiosString;
                        WriteVerbose($"Using automatic SMBIOS settings with profile: {SMBIOSProfile}");
                    }
                }

                client.Put($"nodes/{Node}/qemu/{vmId.Value}/config", configParams);
            }

            // Assign IP if pool is specified
            if (!string.IsNullOrEmpty(IPPool))
            {
                try
                {
                    var ipamManager = new IPAM.IPAMManager();
                    var pool = ipamManager.GetPool(IPPool);
                    var ip = pool.GetNextIP();
                    WriteVerbose($"Assigned IP {ip} from pool {IPPool} to VM {vmName}");
                }
                catch (Exception ex)
                {
                    WriteWarning($"Failed to assign IP from pool {IPPool}: {ex.Message}");
                }
            }

            // Start the VM if requested
            if (Start.IsPresent)
            {
                WriteVerbose($"Starting VM {vmName}");
                client.Post($"nodes/{Node}/qemu/{vmId.Value}/status/start", null);

                // Refresh VM status
                vmResponse = client.Get($"nodes/{Node}/qemu/{vmId.Value}/status/current");
                vm = JsonUtility.DeserializeResponse<ProxmoxVM>(vmResponse);
                vm.Node = Node;
                vm.VMID = vmId.Value;
            }

            return vm;
        }
    }
}
