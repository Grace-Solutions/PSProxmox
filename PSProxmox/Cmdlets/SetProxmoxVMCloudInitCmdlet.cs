using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Security;
using PSProxmox.Client;
using PSProxmox.Models;
using PSProxmox.Session;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Sets Cloud-Init configuration for a VM.</para>
    /// <para type="description">The Set-ProxmoxVMCloudInit cmdlet sets Cloud-Init configuration for a VM.</para>
    /// </summary>
    /// <example>
    ///   <para>Set Cloud-Init configuration for a VM</para>
    ///   <code>Set-ProxmoxVMCloudInit -Node "pve1" -VMID 100 -Username "admin" -Password (ConvertTo-SecureString "password" -AsPlainText -Force) -SSHKey "ssh-rsa AAAA..." -IPConfig "dhcp" -DNS "8.8.8.8,8.8.4.4"</code>
    /// </example>
    /// <example>
    ///   <para>Set Cloud-Init configuration with static IP</para>
    ///   <code>Set-ProxmoxVMCloudInit -Node "pve1" -VMID 100 -Username "admin" -Password (ConvertTo-SecureString "password" -AsPlainText -Force) -IPConfig "ip=192.168.1.100/24,gw=192.168.1.1"</code>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ProxmoxVMCloudInit")]
    [OutputType(typeof(ProxmoxVM))]
    public class SetProxmoxVMCloudInitCmdlet : ProxmoxCmdlet
    {
        /// <summary>
        /// <para type="description">The node on which the VM is located.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public string Node { get; set; }

        /// <summary>
        /// <para type="description">The ID of the VM.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public int VMID { get; set; }

        /// <summary>
        /// <para type="description">The username for Cloud-Init.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Username { get; set; }

        /// <summary>
        /// <para type="description">The password for Cloud-Init.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SecureString Password { get; set; }

        /// <summary>
        /// <para type="description">The SSH public key for Cloud-Init.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string SSHKey { get; set; }

        /// <summary>
        /// <para type="description">The IP configuration for Cloud-Init (e.g., "dhcp" or "ip=192.168.1.100/24,gw=192.168.1.1").</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string IPConfig { get; set; }

        /// <summary>
        /// <para type="description">The DNS servers for Cloud-Init (comma-separated).</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string DNS { get; set; }

        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public new ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = GetProxmoxClient(Connection);

                var parameters = new Dictionary<string, string>();

                // Add Cloud-Init parameters
                if (!string.IsNullOrEmpty(Username))
                {
                    parameters["ciuser"] = Username;
                }

                if (Password != null)
                {
                    parameters["cipassword"] = ConvertSecureStringToString(Password);
                }

                if (!string.IsNullOrEmpty(SSHKey))
                {
                    parameters["sshkeys"] = Uri.EscapeDataString(SSHKey.Replace("\n", "\\n"));
                }

                if (!string.IsNullOrEmpty(IPConfig))
                {
                    parameters["ipconfig0"] = IPConfig;
                }

                if (!string.IsNullOrEmpty(DNS))
                {
                    parameters["nameserver"] = DNS;
                }

                // Set Cloud-Init configuration
                WriteVerbose($"Setting Cloud-Init configuration for VM {VMID} on node {Node}...");

                var response = client.Put($"nodes/{Node}/qemu/{VMID}/config", parameters);

                WriteVerbose("Cloud-Init configuration set successfully");

                // Get the VM details
                var vmResponse = client.Get($"nodes/{Node}/qemu/{VMID}/status/current");
                var vm = Newtonsoft.Json.JsonConvert.DeserializeObject<ProxmoxVM>(vmResponse);

                WriteObject(vm);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(
                    ex,
                    "SetProxmoxVMCloudInitError",
                    ErrorCategory.NotSpecified,
                    null));
            }
        }

        private string ConvertSecureStringToString(SecureString secureString)
        {
            IntPtr valuePtr = IntPtr.Zero;
            try
            {
                valuePtr = System.Runtime.InteropServices.Marshal.SecureStringToGlobalAllocUnicode(secureString);
                return System.Runtime.InteropServices.Marshal.PtrToStringUni(valuePtr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
    }
}
