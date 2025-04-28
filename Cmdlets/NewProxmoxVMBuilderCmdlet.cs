using System;
using System.Management.Automation;
using PSProxmox.Models;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Creates a new virtual machine configuration builder for Proxmox VE.</para>
    /// <para type="description">The New-ProxmoxVMBuilder cmdlet creates a new virtual machine configuration builder that can be used with New-ProxmoxVM.</para>
    /// <example>
    ///   <para>Create a basic VM builder</para>
    ///   <code>$builder = New-ProxmoxVMBuilder -Name "web-server"</code>
    /// </example>
    /// <example>
    ///   <para>Create a VM builder with initial configuration</para>
    ///   <code>$builder = New-ProxmoxVMBuilder -Name "db-server" -Memory 4096 -Cores 2 -Node "pve1"</code>
    /// </example>
    /// <example>
    ///   <para>Create a VM builder and add configuration</para>
    ///   <code>$builder = New-ProxmoxVMBuilder -Name "app-server"
    /// $builder.WithMemory(8192).WithCores(4).WithDisk(100, "local-lvm")
    /// $builder.WithNetwork("virtio", "vmbr0").WithIPConfig("192.168.1.10/24", "192.168.1.1")</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "ProxmoxVMBuilder")]
    [OutputType(typeof(ProxmoxVMBuilder))]
    public class NewProxmoxVMBuilderCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The name of the VM.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">The VM ID. If not specified, the next available ID will be used.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int? VMID { get; set; }

        /// <summary>
        /// <para type="description">The node to create the VM on.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Node { get; set; }

        /// <summary>
        /// <para type="description">The description of the VM.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Description { get; set; }

        /// <summary>
        /// <para type="description">The tags for the VM.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string[] Tags { get; set; }

        /// <summary>
        /// <para type="description">The amount of memory in MB.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int Memory { get; set; } = 512;

        /// <summary>
        /// <para type="description">The number of CPU cores.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int Cores { get; set; } = 1;

        /// <summary>
        /// <para type="description">The CPU type.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string CPUType { get; set; } = "host";

        /// <summary>
        /// <para type="description">The operating system type.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string OSType { get; set; } = "l26"; // Linux 2.6+

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
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var builder = new ProxmoxVMBuilder(Name)
                {
                    Memory = Memory,
                    Cores = Cores,
                    CPUType = CPUType,
                    OSType = OSType,
                    Start = Start.IsPresent,
                    IPPool = IPPool
                };

                if (VMID.HasValue)
                {
                    builder.WithVMID(VMID.Value);
                }

                if (!string.IsNullOrEmpty(Node))
                {
                    builder.WithNode(Node);
                }

                if (!string.IsNullOrEmpty(Description))
                {
                    builder.WithDescription(Description);
                }

                if (Tags != null && Tags.Length > 0)
                {
                    builder.WithTags(Tags);
                }

                WriteObject(builder);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "NewProxmoxVMBuilderError", ErrorCategory.InvalidOperation, Name));
            }
        }
    }
}
