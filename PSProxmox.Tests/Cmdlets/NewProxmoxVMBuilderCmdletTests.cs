using Microsoft.VisualStudio.TestTools.UnitTesting;
using PSProxmox.Cmdlets;
using PSProxmox.Models;
using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PSProxmox.Tests.Cmdlets
{
    [TestClass]
    public class NewProxmoxVMBuilderCmdletTests
    {
        [TestMethod]
        public void ProcessRecord_CreatesBuilder()
        {
            // Arrange
            var cmdlet = new NewProxmoxVMBuilderCmdlet
            {
                Name = "test-vm",
                Memory = 2048,
                Cores = 2,
                CPUType = "host",
                OSType = "l26",
                Start = true,
                IPPool = "test-pool"
            };

            // Act
            ProxmoxVMBuilder result = null;
            using (var ps = PowerShell.Create())
            {
                ps.Runspace = RunspaceFactory.CreateRunspace();
                ps.Runspace.Open();

                // Add the cmdlet to the pipeline
                ps.Commands.AddCommand(cmdlet);

                // Execute the cmdlet
                var results = ps.Invoke();

                // Get the result
                if (results.Count > 0)
                {
                    result = results[0].BaseObject as ProxmoxVMBuilder;
                }
            }

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("test-vm", result.Name);
            Assert.AreEqual(2048, result.Memory);
            Assert.AreEqual(2, result.Cores);
            Assert.AreEqual("host", result.CPUType);
            Assert.AreEqual("l26", result.OSType);
            Assert.IsTrue(result.Start);
            Assert.AreEqual("test-pool", result.IPPool);
        }

        [TestMethod]
        [ExpectedException(typeof(PSArgumentNullException))]
        public void ProcessRecord_ThrowsOnNullName()
        {
            // Arrange
            var cmdlet = new NewProxmoxVMBuilderCmdlet
            {
                Name = null
            };

            // Act
            using (var ps = PowerShell.Create())
            {
                ps.Runspace = RunspaceFactory.CreateRunspace();
                ps.Runspace.Open();

                // Add the cmdlet to the pipeline
                ps.Commands.AddCommand(cmdlet);

                // Execute the cmdlet
                ps.Invoke();
            }
        }

        [TestMethod]
        public void ProcessRecord_WithNode_CreatesBuilder()
        {
            // Arrange
            var cmdlet = new NewProxmoxVMBuilderCmdlet
            {
                Name = "test-vm",
                Node = "pve1"
            };

            // Act
            ProxmoxVMBuilder result = null;
            using (var ps = PowerShell.Create())
            {
                ps.Runspace = RunspaceFactory.CreateRunspace();
                ps.Runspace.Open();

                // Add the cmdlet to the pipeline
                ps.Commands.AddCommand(cmdlet);

                // Execute the cmdlet
                var results = ps.Invoke();

                // Get the result
                if (results.Count > 0)
                {
                    result = results[0].BaseObject as ProxmoxVMBuilder;
                }
            }

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("test-vm", result.Name);
            Assert.AreEqual("pve1", result.Node);
        }

        [TestMethod]
        public void ProcessRecord_WithVMID_CreatesBuilder()
        {
            // Arrange
            var cmdlet = new NewProxmoxVMBuilderCmdlet
            {
                Name = "test-vm",
                VMID = 100
            };

            // Act
            ProxmoxVMBuilder result = null;
            using (var ps = PowerShell.Create())
            {
                ps.Runspace = RunspaceFactory.CreateRunspace();
                ps.Runspace.Open();

                // Add the cmdlet to the pipeline
                ps.Commands.AddCommand(cmdlet);

                // Execute the cmdlet
                var results = ps.Invoke();

                // Get the result
                if (results.Count > 0)
                {
                    result = results[0].BaseObject as ProxmoxVMBuilder;
                }
            }

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("test-vm", result.Name);
            Assert.AreEqual(100, result.VMID);
        }

        [TestMethod]
        public void ProcessRecord_WithDescription_CreatesBuilder()
        {
            // Arrange
            var cmdlet = new NewProxmoxVMBuilderCmdlet
            {
                Name = "test-vm",
                Description = "Test VM"
            };

            // Act
            ProxmoxVMBuilder result = null;
            using (var ps = PowerShell.Create())
            {
                ps.Runspace = RunspaceFactory.CreateRunspace();
                ps.Runspace.Open();

                // Add the cmdlet to the pipeline
                ps.Commands.AddCommand(cmdlet);

                // Execute the cmdlet
                var results = ps.Invoke();

                // Get the result
                if (results.Count > 0)
                {
                    result = results[0].BaseObject as ProxmoxVMBuilder;
                }
            }

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("test-vm", result.Name);
            Assert.AreEqual("Test VM", result.Description);
        }

        [TestMethod]
        public void ProcessRecord_WithTags_CreatesBuilder()
        {
            // Arrange
            var cmdlet = new NewProxmoxVMBuilderCmdlet
            {
                Name = "test-vm",
                Tags = new[] { "tag1", "tag2" }
            };

            // Act
            ProxmoxVMBuilder result = null;
            using (var ps = PowerShell.Create())
            {
                ps.Runspace = RunspaceFactory.CreateRunspace();
                ps.Runspace.Open();

                // Add the cmdlet to the pipeline
                ps.Commands.AddCommand(cmdlet);

                // Execute the cmdlet
                var results = ps.Invoke();

                // Get the result
                if (results.Count > 0)
                {
                    result = results[0].BaseObject as ProxmoxVMBuilder;
                }
            }

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("test-vm", result.Name);
            Assert.AreEqual("tag1,tag2", result.Tags);
        }
    }
}
