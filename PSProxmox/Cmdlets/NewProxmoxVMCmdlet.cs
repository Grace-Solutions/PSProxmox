using System;
using System.Collections.Generic;
using System.Management.Automation;
using Newtonsoft.Json;
using PSProxmox.Client;
using PSProxmox.Models;
using PSProxmox.Session;
using PSProxmox.Utilities;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Creates a new virtual machine in Proxmox VE.</para>
    /// <para type="description">The New-ProxmoxVM cmdlet creates a new virtual machine in Proxmox VE.</para>
    /// <example>
    ///   <para>Create a new virtual machine using direct parameters</para>
    ///   <code>$vm = New-ProxmoxVM -Connection $connection -Node "pve1" -Name "test-vm" -Memory 2048 -Cores 2 -DiskSize 32</code>
    /// </example>
    /// <example>
    ///   <para>Create a new virtual machine using a builder</para>
    ///   <code>$builder = New-ProxmoxVMBuilder -Name "web-server"
    /// $builder.WithMemory(4096).WithCores(2).WithDisk(50, "local-lvm")
    /// $builder.WithNetwork("virtio", "vmbr0").WithIPConfig("192.168.1.10/24", "192.168.1.1")
    /// $vm = New-ProxmoxVM -Connection $connection -Node "pve1" -Builder $builder</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "ProxmoxVM")]
    [OutputType(typeof(ProxmoxVM))]
    public class NewProxmoxVMCmdlet : PSCmdlet
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
        /// <para type="description">The VM configuration builder.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Builder")]
        public ProxmoxVMBuilder Builder { get; set; }

        /// <summary>
        /// <para type="description">The name of the VM.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Direct")]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">The VM ID. If not specified, the next available ID will be used.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Direct")]
        public int? VMID { get; set; }

        /// <summary>
        /// <para type="description">The amount of memory in MB.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Direct")]
        public int Memory { get; set; } = 512;

        /// <summary>
        /// <para type="description">The number of CPU cores.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Direct")]
        public int Cores { get; set; } = 1;

        /// <summary>
        /// <para type="description">The disk size in GB.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Direct")]
        public int DiskSize { get; set; } = 8;

        /// <summary>
        /// <para type="description">The storage location for the disk.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Direct")]
        public string Storage { get; set; } = "local";

        /// <summary>
        /// <para type="description">The operating system type.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Direct")]
        public string OSType { get; set; } = "l26"; // Linux 2.6+

        /// <summary>
        /// <para type="description">The network interface model.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Direct")]
        public string NetworkModel { get; set; } = "virtio";

        /// <summary>
        /// <para type="description">The network bridge.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Direct")]
        public string NetworkBridge { get; set; } = "vmbr0";

        /// <summary>
        /// <para type="description">Whether to start the VM after creation.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Direct")]
        public SwitchParameter Start { get; set; }

        /// <summary>
        /// <para type="description">The IP pool to use for assigning an IP address.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Direct")]
        public string IPPool { get; set; }

        /// <summary>
        /// <para type="description">Whether to automatically generate SMBIOS values.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Direct")]
        public SwitchParameter AutomaticSMBIOS { get; set; }

        /// <summary>
        /// <para type="description">The manufacturer profile to use for SMBIOS values. Valid values are: Proxmox, Dell, HP, Lenovo, VMware, HyperV, VirtualBox, Random.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Direct")]
        [ValidateSet("Proxmox", "Dell", "HP", "Lenovo", "VMware", "HyperV", "VirtualBox", "Random")]
        public string SMBIOSProfile { get; set; } = "Random";

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);
                Dictionary<string, string> parameters;
                int vmid;
                string vmName;
                string ipPool;
                bool startVm;

                // Process based on parameter set
                if (ParameterSetName == "Builder")
                {
                    // Use the builder to create parameters
                    if (Builder == null)
                    {
                        throw new PSArgumentNullException(nameof(Builder));
                    }

                    // Set the node if not already set in the builder
                    if (string.IsNullOrEmpty(Builder.Node))
                    {
                        Builder.WithNode(Node);
                    }

                    // Get the next available VMID if not specified
                    if (!Builder.VMID.HasValue)
                    {
                        string response = client.Get("cluster/nextid");
                        var nextId = JsonUtility.DeserializeResponse<string>(response);
                        Builder.WithVMID(int.Parse(nextId));
                    }

                    // Build the parameters
                    parameters = Builder.Build();
                    vmid = Builder.VMID.Value;
                    vmName = Builder.Name;
                    ipPool = Builder.IPPool;
                    startVm = Builder.Start;
                }
                else // Direct parameters
                {
                    // Get the next available VMID if not specified
                    if (!VMID.HasValue)
                    {
                        string response = client.Get("cluster/nextid");
                        var nextId = JsonUtility.DeserializeResponse<string>(response);
                        VMID = int.Parse(nextId);
                    }

                    // Create the VM parameters
                    parameters = new Dictionary<string, string>
                    {
                        ["vmid"] = VMID.Value.ToString(),
                        ["name"] = Name,
                        ["memory"] = Memory.ToString(),
                        ["cores"] = Cores.ToString(),
                        ["ostype"] = OSType,
                        ["net0"] = $"{NetworkModel},bridge={NetworkBridge}"
                    };

                    // Add disk
                    parameters["ide0"] = $"{Storage}:{DiskSize}";

                    // Add SMBIOS settings if requested
                    if (AutomaticSMBIOS.IsPresent)
                    {
                        var smbios = ProxmoxVMSMBIOSProfile.GetProfile(SMBIOSProfile);
                        string smbiosString = smbios.ToProxmoxString();
                        if (!string.IsNullOrEmpty(smbiosString))
                        {
                            parameters["smbios"] = smbiosString;
                            WriteVerbose($"Using automatic SMBIOS settings with profile: {SMBIOSProfile}");
                        }
                    }

                    vmid = VMID.Value;
                    vmName = Name;
                    ipPool = IPPool;
                    startVm = Start.IsPresent;
                }

                // Create the VM
                WriteVerbose($"Creating VM {vmName} on node {Node}");
                client.Post($"nodes/{Node}/qemu", parameters);

                // Get the created VM
                string vmResponse = client.Get($"nodes/{Node}/qemu/{vmid}/status/current");
                var vm = JsonUtility.DeserializeResponse<ProxmoxVM>(vmResponse);
                vm.Node = Node;
                vm.VMID = vmid;

                // Assign IP if pool is specified
                if (!string.IsNullOrEmpty(ipPool))
                {
                    try
                    {
                        var ipamManager = new IPAM.IPAMManager();
                        var pool = ipamManager.GetPool(ipPool);
                        var ip = pool.GetNextIP();
                        WriteVerbose($"Assigned IP {ip} from pool {ipPool} to VM {vmName}");
                    }
                    catch (Exception ex)
                    {
                        WriteWarning($"Failed to assign IP from pool {ipPool}: {ex.Message}");
                    }
                }

                // Start the VM if requested
                if (startVm)
                {
                    WriteVerbose($"Starting VM {vmName}");
                    client.Post($"nodes/{Node}/qemu/{vmid}/status/start", null);

                    // Refresh VM status
                    vmResponse = client.Get($"nodes/{Node}/qemu/{vmid}/status/current");
                    vm = JsonUtility.DeserializeResponse<ProxmoxVM>(vmResponse);
                    vm.Node = Node;
                    vm.VMID = vmid;
                }

                WriteObject(vm);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "NewProxmoxVMError", ErrorCategory.OperationStopped, Connection));
            }
        }
    }
}
