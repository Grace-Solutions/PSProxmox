using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace PSProxmox.IPAM
{
    /// <summary>
    /// Manages IP address pools for Proxmox VE.
    /// </summary>
    public class IPAMManager
    {
        private readonly Dictionary<string, IPPool> _pools = new Dictionary<string, IPPool>();

        /// <summary>
        /// Creates a new IP pool from a CIDR notation.
        /// </summary>
        /// <param name="name">The name of the pool.</param>
        /// <param name="cidr">The CIDR notation (e.g., 192.168.1.0/24).</param>
        /// <param name="excludeIPs">IPs to exclude from the pool.</param>
        /// <returns>The created IP pool.</returns>
        public IPPool CreatePool(string name, string cidr, IEnumerable<string> excludeIPs = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrEmpty(cidr))
            {
                throw new ArgumentNullException(nameof(cidr));
            }

            if (_pools.ContainsKey(name))
            {
                throw new ArgumentException($"Pool with name '{name}' already exists");
            }

            var pool = new IPPool(name, cidr, excludeIPs);
            _pools[name] = pool;
            return pool;
        }

        /// <summary>
        /// Gets an IP pool by name.
        /// </summary>
        /// <param name="name">The name of the pool.</param>
        /// <returns>The IP pool.</returns>
        public IPPool GetPool(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (!_pools.TryGetValue(name, out var pool))
            {
                throw new KeyNotFoundException($"Pool with name '{name}' not found");
            }

            return pool;
        }

        /// <summary>
        /// Gets all IP pools.
        /// </summary>
        /// <returns>All IP pools.</returns>
        public IEnumerable<IPPool> GetPools()
        {
            return _pools.Values;
        }

        /// <summary>
        /// Removes an IP pool.
        /// </summary>
        /// <param name="name">The name of the pool.</param>
        public void RemovePool(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (!_pools.ContainsKey(name))
            {
                throw new KeyNotFoundException($"Pool with name '{name}' not found");
            }

            _pools.Remove(name);
        }

        /// <summary>
        /// Clears all IP pools.
        /// </summary>
        public void ClearPools()
        {
            _pools.Clear();
        }
    }

    /// <summary>
    /// Represents an IP address pool.
    /// </summary>
    public class IPPool
    {
        private readonly Queue<IPAddress> _availableIPs = new Queue<IPAddress>();
        private readonly HashSet<IPAddress> _usedIPs = new HashSet<IPAddress>();
        private readonly HashSet<IPAddress> _excludedIPs = new HashSet<IPAddress>();

        /// <summary>
        /// Gets the name of the pool.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the CIDR notation of the pool.
        /// </summary>
        public string CIDR { get; }

        /// <summary>
        /// Gets the network address of the pool.
        /// </summary>
        public IPAddress NetworkAddress { get; }

        /// <summary>
        /// Gets the subnet mask of the pool.
        /// </summary>
        public IPAddress SubnetMask { get; }

        /// <summary>
        /// Gets the prefix length of the pool.
        /// </summary>
        public int PrefixLength { get; }

        /// <summary>
        /// Gets the total number of IPs in the pool.
        /// </summary>
        public int TotalIPs { get; }

        /// <summary>
        /// Gets the number of available IPs in the pool.
        /// </summary>
        public int AvailableIPs => _availableIPs.Count;

        /// <summary>
        /// Gets the number of used IPs in the pool.
        /// </summary>
        public int UsedIPs => _usedIPs.Count;

        /// <summary>
        /// Gets the number of excluded IPs in the pool.
        /// </summary>
        public int ExcludedIPs => _excludedIPs.Count;

        /// <summary>
        /// Initializes a new instance of the <see cref="IPPool"/> class.
        /// </summary>
        /// <param name="name">The name of the pool.</param>
        /// <param name="cidr">The CIDR notation (e.g., 192.168.1.0/24).</param>
        /// <param name="excludeIPs">IPs to exclude from the pool.</param>
        public IPPool(string name, string cidr, IEnumerable<string> excludeIPs = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            CIDR = cidr ?? throw new ArgumentNullException(nameof(cidr));

            // Parse CIDR
            string[] parts = cidr.Split('/');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Invalid CIDR format", nameof(cidr));
            }

            if (!IPAddress.TryParse(parts[0], out var ipAddress))
            {
                throw new ArgumentException("Invalid IP address", nameof(cidr));
            }

            if (!int.TryParse(parts[1], out var prefixLength) || prefixLength < 0 || prefixLength > 32)
            {
                throw new ArgumentException("Invalid prefix length", nameof(cidr));
            }

            PrefixLength = prefixLength;
            SubnetMask = GetSubnetMask(prefixLength);
            NetworkAddress = GetNetworkAddress(ipAddress, SubnetMask);
            TotalIPs = (int)Math.Pow(2, 32 - prefixLength) - 2; // Exclude network and broadcast addresses

            // Initialize available IPs
            var broadcastAddress = GetBroadcastAddress(NetworkAddress, SubnetMask);
            var currentIP = IncrementIP(NetworkAddress); // Skip network address

            // Add excluded IPs
            if (excludeIPs != null)
            {
                foreach (var ip in excludeIPs)
                {
                    if (IPAddress.TryParse(ip, out var excludeIP))
                    {
                        _excludedIPs.Add(excludeIP);
                    }
                }
            }

            // Add available IPs
            while (!currentIP.Equals(broadcastAddress))
            {
                if (!_excludedIPs.Contains(currentIP))
                {
                    _availableIPs.Enqueue(currentIP);
                }
                currentIP = IncrementIP(currentIP);
            }
        }

        /// <summary>
        /// Gets the next available IP address from the pool.
        /// </summary>
        /// <returns>The next available IP address.</returns>
        public IPAddress GetNextIP()
        {
            if (_availableIPs.Count == 0)
            {
                throw new InvalidOperationException("No more IPs available in the pool");
            }

            var ip = _availableIPs.Dequeue();
            _usedIPs.Add(ip);
            return ip;
        }

        /// <summary>
        /// Releases an IP address back to the pool.
        /// </summary>
        /// <param name="ip">The IP address to release.</param>
        public void ReleaseIP(IPAddress ip)
        {
            if (ip == null)
            {
                throw new ArgumentNullException(nameof(ip));
            }

            if (!_usedIPs.Contains(ip))
            {
                throw new ArgumentException("IP is not in use", nameof(ip));
            }

            _usedIPs.Remove(ip);
            _availableIPs.Enqueue(ip);
        }

        /// <summary>
        /// Releases an IP address back to the pool.
        /// </summary>
        /// <param name="ip">The IP address to release.</param>
        public void ReleaseIP(string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                throw new ArgumentNullException(nameof(ip));
            }

            if (!IPAddress.TryParse(ip, out var ipAddress))
            {
                throw new ArgumentException("Invalid IP address", nameof(ip));
            }

            ReleaseIP(ipAddress);
        }

        /// <summary>
        /// Gets all used IP addresses in the pool.
        /// </summary>
        /// <returns>All used IP addresses.</returns>
        public IEnumerable<IPAddress> GetUsedIPs()
        {
            return _usedIPs;
        }

        /// <summary>
        /// Gets all available IP addresses in the pool.
        /// </summary>
        /// <returns>All available IP addresses.</returns>
        public IEnumerable<IPAddress> GetAvailableIPs()
        {
            return _availableIPs;
        }

        /// <summary>
        /// Gets all excluded IP addresses in the pool.
        /// </summary>
        /// <returns>All excluded IP addresses.</returns>
        public IEnumerable<IPAddress> GetExcludedIPs()
        {
            return _excludedIPs;
        }

        /// <summary>
        /// Clears all used IPs and returns them to the available pool.
        /// </summary>
        public void Clear()
        {
            foreach (var ip in _usedIPs.ToList())
            {
                _availableIPs.Enqueue(ip);
            }
            _usedIPs.Clear();
        }

        private static IPAddress GetSubnetMask(int prefixLength)
        {
            uint mask = 0xffffffff;
            mask <<= (32 - prefixLength);
            return new IPAddress(BitConverter.GetBytes(mask).Reverse().ToArray());
        }

        private static IPAddress GetNetworkAddress(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipBytes = address.GetAddressBytes();
            byte[] maskBytes = subnetMask.GetAddressBytes();
            byte[] networkBytes = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
            }

            return new IPAddress(networkBytes);
        }

        private static IPAddress GetBroadcastAddress(IPAddress networkAddress, IPAddress subnetMask)
        {
            byte[] ipBytes = networkAddress.GetAddressBytes();
            byte[] maskBytes = subnetMask.GetAddressBytes();
            byte[] broadcastBytes = new byte[4];

            for (int i = 0; i < 4; i++)
            {
                broadcastBytes[i] = (byte)(ipBytes[i] | ~maskBytes[i]);
            }

            return new IPAddress(broadcastBytes);
        }

        private static IPAddress IncrementIP(IPAddress address)
        {
            byte[] bytes = address.GetAddressBytes();

            for (int i = bytes.Length - 1; i >= 0; i--)
            {
                if (bytes[i] == 255)
                {
                    bytes[i] = 0;
                }
                else
                {
                    bytes[i]++;
                    break;
                }
            }

            return new IPAddress(bytes);
        }
    }
}
