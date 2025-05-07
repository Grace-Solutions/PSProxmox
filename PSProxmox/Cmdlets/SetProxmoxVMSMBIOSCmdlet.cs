using System;
using System.Collections.Generic;
using System.Management.Automation;
using PSProxmox.Models;
using PSProxmox.Session;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Sets the SMBIOS settings for a Proxmox VM.</para>
    /// <para type="description">The Set-ProxmoxVMSMBIOS cmdlet sets the SMBIOS settings for a Proxmox VM.</para>
    /// <example>
    ///   <para>Set SMBIOS settings for a VM</para>
    ///   <code>Set-ProxmoxVMSMBIOS -Connection $connection -Node "pve1" -VMID 100 -Manufacturer "Dell Inc." -Product "PowerEdge R740" -Serial "ABC123"</code>
    /// </example>
    /// <example>
    ///   <para>Set SMBIOS settings using a SMBIOS object</para>
    ///   <code>$smbios = New-Object PSProxmox.Models.ProxmoxVMSMBIOS
    /// $smbios.Manufacturer = "Dell Inc."
    /// $smbios.Product = "PowerEdge R740"
    /// $smbios.Serial = "ABC123"
    /// Set-ProxmoxVMSMBIOS -Connection $connection -Node "pve1" -VMID 100 -SMBIOS $smbios</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Set, "ProxmoxVMSMBIOS", SupportsShouldProcess = true)]
    [OutputType(typeof(ProxmoxVMSMBIOS))]
    public class SetProxmoxVMSMBIOSCmdlet : PSCmdlet
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
        /// <para type="description">The SMBIOS settings object.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "SMBIOSObject")]
        public ProxmoxVMSMBIOS SMBIOS { get; set; }

        /// <summary>
        /// <para type="description">The manufacturer information.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// <para type="description">The product information.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public string Product { get; set; }

        /// <summary>
        /// <para type="description">The version information.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public string Version { get; set; }

        /// <summary>
        /// <para type="description">The serial number.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public string Serial { get; set; }

        /// <summary>
        /// <para type="description">The system family.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public string Family { get; set; }

        /// <summary>
        /// <para type="description">The system UUID.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Individual")]
        public string UUID { get; set; }

        /// <summary>
        /// <para type="description">Whether to use a manufacturer profile for SMBIOS values.</para>
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Profile")]
        public SwitchParameter UseProfile { get; set; }

        /// <summary>
        /// <para type="description">The manufacturer profile to use for SMBIOS values. Valid values are: Proxmox, Dell, HP, Lenovo, Microsoft, VMware, HyperV, VirtualBox, Random.</para>
        /// </summary>
        [Parameter(Mandatory = true, ParameterSetName = "Profile")]
        [ValidateSet("Proxmox", "Dell", "HP", "Lenovo", "Microsoft", "VMware", "HyperV", "VirtualBox", "Random")]
        public string Profile { get; set; }

        /// <summary>
        /// <para type="description">Return the updated SMBIOS settings.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter PassThru { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);

                // Create SMBIOS settings
                ProxmoxVMSMBIOS smbiosSettings;

                // Get current settings first (for all parameter sets)
                string response = client.Get($"nodes/{Node}/qemu/{VMID}/config");
                var configData = JsonUtility.DeserializeResponse<dynamic>(response);

                string currentSmbiosString = configData.data.smbios;
                var currentSmbios = string.IsNullOrEmpty(currentSmbiosString)
                    ? new ProxmoxVMSMBIOS()
                    : ProxmoxVMSMBIOS.FromProxmoxString(currentSmbiosString);

                if (ParameterSetName == "SMBIOSObject")
                {
                    smbiosSettings = SMBIOS ?? new ProxmoxVMSMBIOS();

                    // Preserve UUID if it exists and a new one wasn't specified
                    if (string.IsNullOrEmpty(smbiosSettings.UUID) && !string.IsNullOrEmpty(currentSmbios.UUID))
                    {
                        smbiosSettings.UUID = currentSmbios.UUID;
                        WriteVerbose("Preserved existing UUID from VM configuration");
                    }
                }
                else if (ParameterSetName == "Profile")
                {
                    // Use manufacturer profile but preserve existing UUID
                    smbiosSettings = ProxmoxVMSMBIOSProfile.GetProfile(Profile, currentSmbios);
                    WriteVerbose($"Using SMBIOS profile: {Profile}");
                }
                else
                {
                    // Start with current settings
                    smbiosSettings = currentSmbios;

                    // Update with provided values
                    if (!string.IsNullOrEmpty(Manufacturer))
                        smbiosSettings.Manufacturer = Manufacturer;

                    if (!string.IsNullOrEmpty(Product))
                        smbiosSettings.Product = Product;

                    if (!string.IsNullOrEmpty(Version))
                        smbiosSettings.Version = Version;

                    if (!string.IsNullOrEmpty(Serial))
                        smbiosSettings.Serial = Serial;

                    if (!string.IsNullOrEmpty(Family))
                        smbiosSettings.Family = Family;

                    if (!string.IsNullOrEmpty(UUID))
                        smbiosSettings.UUID = UUID;
                }

                // Convert to Proxmox format
                string smbiosString = smbiosSettings.ToProxmoxString();

                if (string.IsNullOrEmpty(smbiosString))
                {
                    WriteWarning("No SMBIOS settings specified. No changes will be made.");
                    return;
                }

                // Confirm the operation
                if (!ShouldProcess($"VM {VMID} on node {Node}", $"Set SMBIOS settings to {smbiosString}"))
                {
                    return;
                }

                // Update the VM configuration
                var parameters = new Dictionary<string, string>
                {
                    ["smbios"] = smbiosString
                };

                client.Put($"nodes/{Node}/qemu/{VMID}/config", parameters);
                WriteVerbose($"SMBIOS settings updated for VM {VMID} on node {Node}");

                // Return the updated settings if requested
                if (PassThru.IsPresent)
                {
                    WriteObject(smbiosSettings);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "SetProxmoxVMSMBIOSError", ErrorCategory.OperationStopped, Connection));
            }
        }
    }
}
