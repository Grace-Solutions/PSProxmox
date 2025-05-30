using System;
using System.Management.Automation;
using Newtonsoft.Json.Linq;
using PSProxmox.Client;
using PSProxmox.Models;
using PSProxmox.Session;
using PSProxmox.Utilities;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Gets the SMBIOS settings for a Proxmox VM.</para>
    /// <para type="description">The Get-ProxmoxVMSMBIOS cmdlet retrieves the SMBIOS settings for a Proxmox VM.</para>
    /// <example>
    ///   <para>Get SMBIOS settings for a VM</para>
    ///   <code>Get-ProxmoxVMSMBIOS -Connection $connection -Node "pve1" -VMID 100</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "ProxmoxVMSMBIOS")]
    [OutputType(typeof(ProxmoxVMSMBIOS))]
    public class GetProxmoxVMSMBIOSCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The node where the VM is located.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 1)]
        public string Node { get; set; }

        /// <summary>
        /// <para type="description">The ID of the VM.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 2)]
        public int VMID { get; set; }

        /// <summary>
        /// <para type="description">Return the raw JSON response.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter RawJson { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);

                // Get the VM configuration
                string response = client.Get($"nodes/{Node}/qemu/{VMID}/config");
                var configData = JsonUtility.DeserializeResponse<JObject>(response);

                if (RawJson.IsPresent)
                {
                    WriteObject(response);
                    return;
                }

                // Extract SMBIOS settings
                var smbios = new ProxmoxVMSMBIOS();
                if (configData != null && configData.TryGetValue("smbios1", out JToken smbiosToken))
                {
                    string smbiosString = smbiosToken.ToString();
                    smbios = ProxmoxVMSMBIOS.FromProxmoxString(smbiosString);
                }

                WriteObject(smbios);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetProxmoxVMSMBIOSError", ErrorCategory.OperationStopped, Connection));
            }
        }
    }
}
