using System;
using System.Management.Automation;
using PSProxmox.IPAM;
using PSProxmox.Models;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Clears an IP address pool.</para>
    /// <para type="description">The Clear-ProxmoxIPPool cmdlet clears all used IP addresses from an IP pool and returns them to the available pool.</para>
    /// <example>
    ///   <para>Clear a specific IP pool</para>
    ///   <code>Clear-ProxmoxIPPool -Name "Production"</code>
    /// </example>
    /// <example>
    ///   <para>Clear all IP pools</para>
    ///   <code>Clear-ProxmoxIPPool -All</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Clear, "ProxmoxIPPool")]
    [OutputType(typeof(ProxmoxIPPool))]
    public class ClearProxmoxIPPoolCmdlet : PSCmdlet
    {
        private static readonly IPAMManager _ipamManager = new IPAMManager();

        /// <summary>
        /// <para type="description">The name of the IP pool to clear.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "ByName")]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">Whether to clear all IP pools.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "All")]
        public SwitchParameter All { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                if (ParameterSetName == "ByName")
                {
                    // Clear a specific pool
                    var pool = _ipamManager.GetPool(Name);
                    pool.Clear();
                    WriteObject(ProxmoxIPPool.FromIPPool(pool));
                }
                else
                {
                    // Clear all pools
                    foreach (var pool in _ipamManager.GetPools())
                    {
                        pool.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "ClearProxmoxIPPoolError", ErrorCategory.InvalidOperation, Name));
            }
        }
    }
}
