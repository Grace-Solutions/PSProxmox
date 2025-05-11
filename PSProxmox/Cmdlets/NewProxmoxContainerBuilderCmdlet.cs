using System.Management.Automation;
using PSProxmox.Models;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Creates a new Proxmox container builder</para>
    /// <para type="description">Creates a new Proxmox container builder for creating LXC containers</para>
    /// </summary>
    /// <example>
    /// <code>$builder = New-ProxmoxContainerBuilder -Name "web-container"
    /// $builder.WithOSTemplate("local:vztmpl/ubuntu-20.04-standard_20.04-1_amd64.tar.gz")
    ///         .WithStorage("local-lvm")
    ///         .WithMemory(512)
    ///         .WithSwap(512)
    ///         .WithCores(1)
    ///         .WithDiskSize(8)
    ///         .WithUnprivileged($true)
    ///         .WithStartOnBoot($true)
    ///         .WithStart($true)
    /// New-ProxmoxContainer -Node "pve1" -Builder $builder</code>
    /// <para>Creates a new container builder and uses it to create a container</para>
    /// </example>
    [Cmdlet(VerbsCommon.New, "ProxmoxContainerBuilder")]
    [OutputType(typeof(ProxmoxContainerBuilder))]
    public class NewProxmoxContainerBuilderCmdlet : PSCmdlet
    {
        /// <summary>
        /// Gets or sets the container name
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string Name { get; set; }

        /// <summary>
        /// Processes the cmdlet
        /// </summary>
        protected override void ProcessRecord()
        {
            var builder = new ProxmoxContainerBuilder(Name);
            WriteObject(builder);
        }
    }
}
