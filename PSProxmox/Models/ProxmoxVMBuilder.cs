using System;
using System.Collections.Generic;
using System.Linq;

namespace PSProxmox.Models
{
    /// <summary>
    /// Builder for creating virtual machine configurations in Proxmox VE.
    /// </summary>
    public class ProxmoxVMBuilder
    {
        private readonly Dictionary<string, string> _parameters = new Dictionary<string, string>();
        private readonly List<string> _networkInterfaces = new List<string>();
        private readonly List<string> _disks = new List<string>();
        private readonly List<string> _serialPorts = new List<string>();
        private readonly List<string> _usbDevices = new List<string>();
        private readonly List<string> _pciDevices = new List<string>();

        /// <summary>
        /// Gets or sets the name of the VM.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the VM ID.
        /// </summary>
        public int? VMID { get; set; }

        /// <summary>
        /// Gets or sets the node where the VM will be created.
        /// </summary>
        public string Node { get; set; }

        /// <summary>
        /// Gets or sets the description of the VM.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the tags for the VM.
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Gets or sets the memory in MB.
        /// </summary>
        public int Memory { get; set; } = 512;

        /// <summary>
        /// Gets or sets the number of CPU cores.
        /// </summary>
        public int Cores { get; set; } = 1;

        /// <summary>
        /// Gets or sets the CPU type.
        /// </summary>
        public string CPUType { get; set; } = "host";

        /// <summary>
        /// Gets or sets the operating system type.
        /// </summary>
        public string OSType { get; set; } = "l26"; // Linux 2.6+

        /// <summary>
        /// Gets or sets a value indicating whether to start the VM after creation.
        /// </summary>
        public bool Start { get; set; }

        /// <summary>
        /// Gets or sets the IP pool to use for assigning an IP address.
        /// </summary>
        public string IPPool { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxmoxVMBuilder"/> class.
        /// </summary>
        /// <param name="name">The name of the VM.</param>
        public ProxmoxVMBuilder(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Sets the VM ID.
        /// </summary>
        /// <param name="vmid">The VM ID.</param>
        /// <returns>The builder instance.</returns>
        public ProxmoxVMBuilder WithVMID(int vmid)
        {
            VMID = vmid;
            return this;
        }

        /// <summary>
        /// Sets the node where the VM will be created.
        /// </summary>
        /// <param name="node">The node name.</param>
        /// <returns>The builder instance.</returns>
        public ProxmoxVMBuilder WithNode(string node)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
            return this;
        }

        /// <summary>
        /// Sets the description of the VM.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns>The builder instance.</returns>
        public ProxmoxVMBuilder WithDescription(string description)
        {
            Description = description;
            return this;
        }

        /// <summary>
        /// Sets the tags for the VM.
        /// </summary>
        /// <param name="tags">The tags.</param>
        /// <returns>The builder instance.</returns>
        public ProxmoxVMBuilder WithTags(params string[] tags)
        {
            Tags = string.Join(",", tags);
            return this;
        }

        /// <summary>
        /// Sets the memory for the VM.
        /// </summary>
        /// <param name="memoryMB">The memory in MB.</param>
        /// <returns>The builder instance.</returns>
        public ProxmoxVMBuilder WithMemory(int memoryMB)
        {
            if (memoryMB <= 0)
            {
                throw new ArgumentException("Memory must be greater than 0", nameof(memoryMB));
            }

            Memory = memoryMB;
            return this;
        }

        /// <summary>
        /// Sets the number of CPU cores for the VM.
        /// </summary>
        /// <param name="cores">The number of cores.</param>
        /// <returns>The builder instance.</returns>
        public ProxmoxVMBuilder WithCores(int cores)
        {
            if (cores <= 0)
            {
                throw new ArgumentException("Cores must be greater than 0", nameof(cores));
            }

            Cores = cores;
            return this;
        }

        /// <summary>
        /// Sets the CPU type for the VM.
        /// </summary>
        /// <param name="cpuType">The CPU type.</param>
        /// <returns>The builder instance.</returns>
        public ProxmoxVMBuilder WithCPUType(string cpuType)
        {
            CPUType = cpuType ?? throw new ArgumentNullException(nameof(cpuType));
            return this;
        }

        /// <summary>
        /// Sets the operating system type for the VM.
        /// </summary>
        /// <param name="osType">The OS type.</param>
        /// <returns>The builder instance.</returns>
        public ProxmoxVMBuilder WithOSType(string osType)
        {
            OSType = osType ?? throw new ArgumentNullException(nameof(osType));
            return this;
        }

        /// <summary>
        /// Sets whether to start the VM after creation.
        /// </summary>
        /// <param name="start">Whether to start the VM.</param>
        /// <returns>The builder instance.</returns>
        public ProxmoxVMBuilder WithStart(bool start)
        {
            Start = start;
            return this;
        }

        /// <summary>
        /// Sets the IP pool to use for assigning an IP address.
        /// </summary>
        /// <param name="ipPool">The IP pool name.</param>
        /// <returns>The builder instance.</returns>
        public ProxmoxVMBuilder WithIPPool(string ipPool)
        {
            IPPool = ipPool;
            return this;
        }

        /// <summary>
        /// Adds a disk to the VM.
        /// </summary>
        /// <param name="sizeGB">The disk size in GB.</param>
        /// <param name="storage">The storage location.</param>
        /// <param name="format">The disk format.</param>
        /// <param name="bus">The disk bus.</param>
        /// <returns>The builder instance.</returns>
        public ProxmoxVMBuilder WithDisk(int sizeGB, string storage, string format = "raw", string bus = "virtio")
        {
            if (sizeGB <= 0)
            {
                throw new ArgumentException("Size must be greater than 0", nameof(sizeGB));
            }

            if (string.IsNullOrEmpty(storage))
            {
                throw new ArgumentNullException(nameof(storage));
            }

            string diskId = $"scsi{_disks.Count}";
            _disks.Add($"{diskId}={storage}:{sizeGB},format={format}");
            return this;
        }

        /// <summary>
        /// Adds a network interface to the VM.
        /// </summary>
        /// <param name="model">The network model.</param>
        /// <param name="bridge">The network bridge.</param>
        /// <param name="vlan">The VLAN ID.</param>
        /// <param name="macAddress">The MAC address.</param>
        /// <returns>The builder instance.</returns>
        public ProxmoxVMBuilder WithNetwork(string model = "virtio", string bridge = "vmbr0", int? vlan = null, string macAddress = null)
        {
            if (string.IsNullOrEmpty(model))
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (string.IsNullOrEmpty(bridge))
            {
                throw new ArgumentNullException(nameof(bridge));
            }

            string netId = $"net{_networkInterfaces.Count}";
            string netConfig = $"{model},bridge={bridge}";

            if (vlan.HasValue)
            {
                netConfig += $",tag={vlan.Value}";
            }

            if (!string.IsNullOrEmpty(macAddress))
            {
                netConfig += $",macaddr={macAddress}";
            }

            _networkInterfaces.Add($"{netId}={netConfig}");
            return this;
        }

        /// <summary>
        /// Adds a static IP configuration to the VM.
        /// </summary>
        /// <param name="ipAddress">The IP address with CIDR notation.</param>
        /// <param name="gateway">The gateway address.</param>
        /// <param name="networkIndex">The network interface index.</param>
        /// <returns>The builder instance.</returns>
        public ProxmoxVMBuilder WithIPConfig(string ipAddress, string gateway, int networkIndex = 0)
        {
            if (string.IsNullOrEmpty(ipAddress))
            {
                throw new ArgumentNullException(nameof(ipAddress));
            }

            if (string.IsNullOrEmpty(gateway))
            {
                throw new ArgumentNullException(nameof(gateway));
            }

            if (networkIndex < 0 || networkIndex >= _networkInterfaces.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(networkIndex));
            }

            string netId = $"net{networkIndex}";
            string netConfig = _networkInterfaces[networkIndex].Split('=')[1];
            _networkInterfaces[networkIndex] = $"{netId}={netConfig},ip={ipAddress},gw={gateway}";
            return this;
        }

        /// <summary>
        /// Sets the boot order for the VM.
        /// </summary>
        /// <param name="bootDevices">The boot devices in order.</param>
        /// <returns>The builder instance.</returns>
        public ProxmoxVMBuilder WithBootOrder(params string[] bootDevices)
        {
            if (bootDevices == null || bootDevices.Length == 0)
            {
                throw new ArgumentException("At least one boot device must be specified", nameof(bootDevices));
            }

            _parameters["boot"] = string.Join(";", bootDevices);
            return this;
        }

        /// <summary>
        /// Sets the VGA type for the VM.
        /// </summary>
        /// <param name="vgaType">The VGA type.</param>
        /// <returns>The builder instance.</returns>
        public ProxmoxVMBuilder WithVGA(string vgaType)
        {
            if (string.IsNullOrEmpty(vgaType))
            {
                throw new ArgumentNullException(nameof(vgaType));
            }

            _parameters["vga"] = vgaType;
            return this;
        }

        /// <summary>
        /// Sets a custom parameter for the VM.
        /// </summary>
        /// <param name="name">The parameter name.</param>
        /// <param name="value">The parameter value.</param>
        /// <returns>The builder instance.</returns>
        public ProxmoxVMBuilder WithParameter(string name, string value)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            _parameters[name] = value;
            return this;
        }

        /// <summary>
        /// Builds the VM configuration parameters.
        /// </summary>
        /// <returns>The VM configuration parameters.</returns>
        public Dictionary<string, string> Build()
        {
            var parameters = new Dictionary<string, string>(_parameters);

            // Add basic parameters
            parameters["name"] = Name;
            parameters["memory"] = Memory.ToString();
            parameters["cores"] = Cores.ToString();
            parameters["cpu"] = CPUType;
            parameters["ostype"] = OSType;

            if (!string.IsNullOrEmpty(Description))
            {
                parameters["description"] = Description;
            }

            if (!string.IsNullOrEmpty(Tags))
            {
                parameters["tags"] = Tags;
            }

            // Add network interfaces
            for (int i = 0; i < _networkInterfaces.Count; i++)
            {
                string[] parts = _networkInterfaces[i].Split('=');
                parameters[parts[0]] = parts[1];
            }

            // Add disks
            for (int i = 0; i < _disks.Count; i++)
            {
                string[] parts = _disks[i].Split('=');
                parameters[parts[0]] = parts[1];
            }

            // Add serial ports
            for (int i = 0; i < _serialPorts.Count; i++)
            {
                string[] parts = _serialPorts[i].Split('=');
                parameters[parts[0]] = parts[1];
            }

            // Add USB devices
            for (int i = 0; i < _usbDevices.Count; i++)
            {
                string[] parts = _usbDevices[i].Split('=');
                parameters[parts[0]] = parts[1];
            }

            // Add PCI devices
            for (int i = 0; i < _pciDevices.Count; i++)
            {
                string[] parts = _pciDevices[i].Split('=');
                parameters[parts[0]] = parts[1];
            }

            return parameters;
        }
    }
}
