using System;
using System.Management.Automation;
using PSProxmox.IPAM;
using PSProxmox.Models;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Creates a new IP address pool.</para>
    /// <para type="description">The New-ProxmoxIPPool cmdlet creates a new IP address pool for use with Proxmox VE virtual machines.</para>
    /// <example>
    ///   <para>Create a new IP pool</para>
    ///   <code>$pool = New-ProxmoxIPPool -Name "Production" -CIDR "192.168.1.0/24" -ExcludeIPs "192.168.1.1", "192.168.1.254"</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "ProxmoxIPPool")]
    [OutputType(typeof(ProxmoxIPPool))]
    public class NewProxmoxIPPoolCmdlet : PSCmdlet
    {
        private static readonly IPAMManager _ipamManager = new IPAMManager();

        /// <summary>
        /// <para type="description">The name of the IP pool.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">The CIDR notation of the IP pool (e.g., 192.168.1.0/24).</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public string CIDR { get; set; }

        /// <summary>
        /// <para type="description">IP addresses to exclude from the pool.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string[] ExcludeIPs { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var pool = _ipamManager.CreatePool(Name, CIDR, ExcludeIPs);
                WriteObject(ProxmoxIPPool.FromIPPool(pool));
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "NewProxmoxIPPoolError", ErrorCategory.InvalidOperation, Name));
            }
        }
    }
}
