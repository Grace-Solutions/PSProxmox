using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSProxmox.IPAM;
using System;
using System.Linq;
using System.Net;

namespace PSProxmox.Tests.IPAM
{
    [TestClass]
    public class IPAMManagerTests
    {
        [TestMethod]
        public void CreatePool_CreatesPool()
        {
            // Arrange
            var ipamManager = new IPAMManager();
            string name = "test-pool";
            string cidr = "192.168.1.0/24";

            // Act
            var pool = ipamManager.CreatePool(name, cidr);

            // Assert
            Assert.AreEqual(name, pool.Name);
            Assert.AreEqual(cidr, pool.CIDR);
            Assert.AreEqual(254, pool.TotalIPs); // 256 - 2 (network and broadcast)
            Assert.AreEqual(254, pool.AvailableIPs);
            Assert.AreEqual(0, pool.UsedIPs);
        }

        [TestMethod]
        public void CreatePool_WithExcludedIPs_CreatesPool()
        {
            // Arrange
            var ipamManager = new IPAMManager();
            string name = "test-pool";
            string cidr = "192.168.1.0/24";
            string[] excludeIPs = new[] { "192.168.1.1", "192.168.1.254" };

            // Act
            var pool = ipamManager.CreatePool(name, cidr, excludeIPs);

            // Assert
            Assert.AreEqual(name, pool.Name);
            Assert.AreEqual(cidr, pool.CIDR);
            Assert.AreEqual(254, pool.TotalIPs);
            Assert.AreEqual(252, pool.AvailableIPs); // 254 - 2 excluded
            Assert.AreEqual(0, pool.UsedIPs);
            Assert.AreEqual(2, pool.ExcludedIPs);
        }

        [TestMethod]
        public void GetPool_ReturnsPool()
        {
            // Arrange
            var ipamManager = new IPAMManager();
            string name = "test-pool";
            string cidr = "192.168.1.0/24";
            ipamManager.CreatePool(name, cidr);

            // Act
            var pool = ipamManager.GetPool(name);

            // Assert
            Assert.AreEqual(name, pool.Name);
            Assert.AreEqual(cidr, pool.CIDR);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetPool_ThrowsOnNonExistentPool()
        {
            // Arrange
            var ipamManager = new IPAMManager();

            // Act
            var pool = ipamManager.GetPool("non-existent-pool");
        }

        [TestMethod]
        public void GetPools_ReturnsAllPools()
        {
            // Arrange
            var ipamManager = new IPAMManager();
            ipamManager.CreatePool("pool1", "192.168.1.0/24");
            ipamManager.CreatePool("pool2", "192.168.2.0/24");

            // Act
            var pools = ipamManager.GetPools().ToList();

            // Assert
            Assert.AreEqual(2, pools.Count);
            Assert.IsTrue(pools.Any(p => p.Name == "pool1"));
            Assert.IsTrue(pools.Any(p => p.Name == "pool2"));
        }

        [TestMethod]
        public void RemovePool_RemovesPool()
        {
            // Arrange
            var ipamManager = new IPAMManager();
            ipamManager.CreatePool("pool1", "192.168.1.0/24");
            ipamManager.CreatePool("pool2", "192.168.2.0/24");

            // Act
            ipamManager.RemovePool("pool1");
            var pools = ipamManager.GetPools().ToList();

            // Assert
            Assert.AreEqual(1, pools.Count);
            Assert.IsTrue(pools.Any(p => p.Name == "pool2"));
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void RemovePool_ThrowsOnNonExistentPool()
        {
            // Arrange
            var ipamManager = new IPAMManager();

            // Act
            ipamManager.RemovePool("non-existent-pool");
        }

        [TestMethod]
        public void ClearPools_ClearsAllPools()
        {
            // Arrange
            var ipamManager = new IPAMManager();
            ipamManager.CreatePool("pool1", "192.168.1.0/24");
            ipamManager.CreatePool("pool2", "192.168.2.0/24");

            // Act
            ipamManager.ClearPools();
            var pools = ipamManager.GetPools().ToList();

            // Assert
            Assert.AreEqual(0, pools.Count);
        }
    }

    [TestClass]
    public class IPPoolTests
    {
        [TestMethod]
        public void Constructor_InitializesPool()
        {
            // Arrange
            string name = "test-pool";
            string cidr = "192.168.1.0/24";

            // Act
            var pool = new IPPool(name, cidr);

            // Assert
            Assert.AreEqual(name, pool.Name);
            Assert.AreEqual(cidr, pool.CIDR);
            Assert.AreEqual(IPAddress.Parse("192.168.1.0"), pool.NetworkAddress);
            Assert.AreEqual(IPAddress.Parse("255.255.255.0"), pool.SubnetMask);
            Assert.AreEqual(24, pool.PrefixLength);
            Assert.AreEqual(254, pool.TotalIPs);
            Assert.AreEqual(254, pool.AvailableIPs);
            Assert.AreEqual(0, pool.UsedIPs);
        }

        [TestMethod]
        public void Constructor_WithExcludedIPs_InitializesPool()
        {
            // Arrange
            string name = "test-pool";
            string cidr = "192.168.1.0/24";
            string[] excludeIPs = new[] { "192.168.1.1", "192.168.1.254" };

            // Act
            var pool = new IPPool(name, cidr, excludeIPs);

            // Assert
            Assert.AreEqual(name, pool.Name);
            Assert.AreEqual(cidr, pool.CIDR);
            Assert.AreEqual(254, pool.TotalIPs);
            Assert.AreEqual(252, pool.AvailableIPs);
            Assert.AreEqual(0, pool.UsedIPs);
            Assert.AreEqual(2, pool.ExcludedIPs);
        }

        [TestMethod]
        public void GetNextIP_ReturnsNextIP()
        {
            // Arrange
            var pool = new IPPool("test-pool", "192.168.1.0/24");

            // Act
            var ip1 = pool.GetNextIP();
            var ip2 = pool.GetNextIP();

            // Assert
            Assert.AreEqual(IPAddress.Parse("192.168.1.1"), ip1);
            Assert.AreEqual(IPAddress.Parse("192.168.1.2"), ip2);
            Assert.AreEqual(252, pool.AvailableIPs);
            Assert.AreEqual(2, pool.UsedIPs);
        }

        [TestMethod]
        public void ReleaseIP_ReleasesIP()
        {
            // Arrange
            var pool = new IPPool("test-pool", "192.168.1.0/24");
            var ip1 = pool.GetNextIP();
            var ip2 = pool.GetNextIP();

            // Act
            pool.ReleaseIP(ip1);

            // Assert
            Assert.AreEqual(253, pool.AvailableIPs);
            Assert.AreEqual(1, pool.UsedIPs);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ReleaseIP_ThrowsOnUnusedIP()
        {
            // Arrange
            var pool = new IPPool("test-pool", "192.168.1.0/24");
            var ip = IPAddress.Parse("192.168.1.10");

            // Act
            pool.ReleaseIP(ip);
        }

        [TestMethod]
        public void Clear_ClearsAllUsedIPs()
        {
            // Arrange
            var pool = new IPPool("test-pool", "192.168.1.0/24");
            pool.GetNextIP();
            pool.GetNextIP();

            // Act
            pool.Clear();

            // Assert
            Assert.AreEqual(254, pool.AvailableIPs);
            Assert.AreEqual(0, pool.UsedIPs);
        }
    }
}
