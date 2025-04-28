using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace PSProxmox.Models
{
    /// <summary>
    /// Represents an IP address pool for Proxmox VE.
    /// </summary>
    public class ProxmoxIPPool
    {
        /// <summary>
        /// Gets or sets the name of the pool.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the CIDR notation of the pool.
        /// </summary>
        public string CIDR { get; set; }

        /// <summary>
        /// Gets or sets the network address of the pool.
        /// </summary>
        public string NetworkAddress { get; set; }

        /// <summary>
        /// Gets or sets the subnet mask of the pool.
        /// </summary>
        public string SubnetMask { get; set; }

        /// <summary>
        /// Gets or sets the prefix length of the pool.
        /// </summary>
        public int PrefixLength { get; set; }

        /// <summary>
        /// Gets or sets the total number of IPs in the pool.
        /// </summary>
        public int TotalIPs { get; set; }

        /// <summary>
        /// Gets or sets the number of available IPs in the pool.
        /// </summary>
        public int AvailableIPs { get; set; }

        /// <summary>
        /// Gets or sets the number of used IPs in the pool.
        /// </summary>
        public int UsedIPs { get; set; }

        /// <summary>
        /// Gets or sets the number of excluded IPs in the pool.
        /// </summary>
        public int ExcludedIPs { get; set; }

        /// <summary>
        /// Gets or sets the list of used IP addresses.
        /// </summary>
        public List<string> UsedIPAddresses { get; set; }

        /// <summary>
        /// Gets or sets the list of available IP addresses.
        /// </summary>
        public List<string> AvailableIPAddresses { get; set; }

        /// <summary>
        /// Gets or sets the list of excluded IP addresses.
        /// </summary>
        public List<string> ExcludedIPAddresses { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="ProxmoxIPPool"/> class from an <see cref="IPAM.IPPool"/> object.
        /// </summary>
        /// <param name="pool">The IP pool.</param>
        /// <returns>A new IP pool information object.</returns>
        public static ProxmoxIPPool FromIPPool(IPAM.IPPool pool)
        {
            return new ProxmoxIPPool
            {
                Name = pool.Name,
                CIDR = pool.CIDR,
                NetworkAddress = pool.NetworkAddress.ToString(),
                SubnetMask = pool.SubnetMask.ToString(),
                PrefixLength = pool.PrefixLength,
                TotalIPs = pool.TotalIPs,
                AvailableIPs = pool.AvailableIPs,
                UsedIPs = pool.UsedIPs,
                ExcludedIPs = pool.ExcludedIPs,
                UsedIPAddresses = pool.GetUsedIPs().Select(ip => ip.ToString()).ToList(),
                AvailableIPAddresses = pool.GetAvailableIPs().Select(ip => ip.ToString()).ToList(),
                ExcludedIPAddresses = pool.GetExcludedIPs().Select(ip => ip.ToString()).ToList()
            };
        }
    }
}
