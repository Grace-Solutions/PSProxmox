using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSProxmox.Models;
using System;
using System.Collections.Generic;

namespace PSProxmox.Tests.Models
{
    [TestClass]
    public class ProxmoxVMBuilderTests
    {
        [TestMethod]
        public void Constructor_SetsName()
        {
            // Arrange
            string name = "test-vm";

            // Act
            var builder = new ProxmoxVMBuilder(name);

            // Assert
            Assert.AreEqual(name, builder.Name);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ThrowsOnNullName()
        {
            // Act
            var builder = new ProxmoxVMBuilder(null);
        }

        [TestMethod]
        public void WithVMID_SetsVMID()
        {
            // Arrange
            var builder = new ProxmoxVMBuilder("test-vm");
            int vmid = 100;

            // Act
            builder.WithVMID(vmid);

            // Assert
            Assert.AreEqual(vmid, builder.VMID);
        }

        [TestMethod]
        public void WithNode_SetsNode()
        {
            // Arrange
            var builder = new ProxmoxVMBuilder("test-vm");
            string node = "pve1";

            // Act
            builder.WithNode(node);

            // Assert
            Assert.AreEqual(node, builder.Node);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void WithNode_ThrowsOnNullNode()
        {
            // Arrange
            var builder = new ProxmoxVMBuilder("test-vm");

            // Act
            builder.WithNode(null);
        }

        [TestMethod]
        public void WithMemory_SetsMemory()
        {
            // Arrange
            var builder = new ProxmoxVMBuilder("test-vm");
            int memory = 2048;

            // Act
            builder.WithMemory(memory);

            // Assert
            Assert.AreEqual(memory, builder.Memory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WithMemory_ThrowsOnZeroMemory()
        {
            // Arrange
            var builder = new ProxmoxVMBuilder("test-vm");

            // Act
            builder.WithMemory(0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WithMemory_ThrowsOnNegativeMemory()
        {
            // Arrange
            var builder = new ProxmoxVMBuilder("test-vm");

            // Act
            builder.WithMemory(-1);
        }

        [TestMethod]
        public void WithCores_SetsCores()
        {
            // Arrange
            var builder = new ProxmoxVMBuilder("test-vm");
            int cores = 4;

            // Act
            builder.WithCores(cores);

            // Assert
            Assert.AreEqual(cores, builder.Cores);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void WithCores_ThrowsOnZeroCores()
        {
            // Arrange
            var builder = new ProxmoxVMBuilder("test-vm");

            // Act
            builder.WithCores(0);
        }

        [TestMethod]
        public void WithDisk_AddsDisk()
        {
            // Arrange
            var builder = new ProxmoxVMBuilder("test-vm");
            int sizeGB = 32;
            string storage = "local-lvm";

            // Act
            builder.WithDisk(sizeGB, storage);
            var parameters = builder.Build();

            // Assert
            Assert.IsTrue(parameters.ContainsKey("scsi0"));
            Assert.AreEqual($"{storage}:{sizeGB},format=raw", parameters["scsi0"]);
        }

        [TestMethod]
        public void WithNetwork_AddsNetwork()
        {
            // Arrange
            var builder = new ProxmoxVMBuilder("test-vm");
            string model = "virtio";
            string bridge = "vmbr0";

            // Act
            builder.WithNetwork(model, bridge);
            var parameters = builder.Build();

            // Assert
            Assert.IsTrue(parameters.ContainsKey("net0"));
            Assert.AreEqual($"{model},bridge={bridge}", parameters["net0"]);
        }

        [TestMethod]
        public void WithIPConfig_AddsIPConfig()
        {
            // Arrange
            var builder = new ProxmoxVMBuilder("test-vm");
            string model = "virtio";
            string bridge = "vmbr0";
            string ipAddress = "192.168.1.10/24";
            string gateway = "192.168.1.1";

            // Act
            builder.WithNetwork(model, bridge);
            builder.WithIPConfig(ipAddress, gateway);
            var parameters = builder.Build();

            // Assert
            Assert.IsTrue(parameters.ContainsKey("net0"));
            Assert.AreEqual($"{model},bridge={bridge},ip={ipAddress},gw={gateway}", parameters["net0"]);
        }

        [TestMethod]
        public void Build_ReturnsCorrectParameters()
        {
            // Arrange
            var builder = new ProxmoxVMBuilder("test-vm")
                .WithVMID(100)
                .WithNode("pve1")
                .WithMemory(2048)
                .WithCores(2)
                .WithDisk(32, "local-lvm")
                .WithNetwork("virtio", "vmbr0")
                .WithIPConfig("192.168.1.10/24", "192.168.1.1")
                .WithBootOrder("disk", "net")
                .WithVGA("std");

            // Act
            var parameters = builder.Build();

            // Assert
            Assert.AreEqual("test-vm", parameters["name"]);
            Assert.AreEqual("100", parameters["vmid"]);
            Assert.AreEqual("2048", parameters["memory"]);
            Assert.AreEqual("2", parameters["cores"]);
            Assert.AreEqual("host", parameters["cpu"]);
            Assert.AreEqual("l26", parameters["ostype"]);
            Assert.AreEqual("local-lvm:32,format=raw", parameters["scsi0"]);
            Assert.AreEqual("virtio,bridge=vmbr0,ip=192.168.1.10/24,gw=192.168.1.1", parameters["net0"]);
            Assert.AreEqual("disk;net", parameters["boot"]);
            Assert.AreEqual("std", parameters["vga"]);
        }
    }
}
