using System;
using System.Collections.Generic;
using System.Management.Automation;
using PSProxmox.IPAM;
using PSProxmox.Models;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Gets IP address pools.</para>
    /// <para type="description">The Get-ProxmoxIPPool cmdlet retrieves IP address pools used with Proxmox VE virtual machines.</para>
    /// <example>
    ///   <para>Get all IP pools</para>
    ///   <code>$pools = Get-ProxmoxIPPool</code>
    /// </example>
    /// <example>
    ///   <para>Get a specific IP pool by name</para>
    ///   <code>$pool = Get-ProxmoxIPPool -Name "Production"</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "ProxmoxIPPool")]
    [OutputType(typeof(ProxmoxIPPool))]
    public class GetProxmoxIPPoolCmdlet : PSCmdlet
    {
        private static readonly IPAMManager _ipamManager = new IPAMManager();

        /// <summary>
        /// <para type="description">The name of the IP pool to retrieve.</para>
        /// </summary>
        [Parameter(Mandatory = false, Position = 0)]
        public string Name { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                if (string.IsNullOrEmpty(Name))
                {
                    // Get all pools
                    var pools = new List<ProxmoxIPPool>();
                    foreach (var pool in _ipamManager.GetPools())
                    {
                        pools.Add(ProxmoxIPPool.FromIPPool(pool));
                    }
                    WriteObject(pools, true);
                }
                else
                {
                    // Get a specific pool
                    var pool = _ipamManager.GetPool(Name);
                    WriteObject(ProxmoxIPPool.FromIPPool(pool));
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetProxmoxIPPoolError", ErrorCategory.InvalidOperation, Name));
            }
        }
    }
}
