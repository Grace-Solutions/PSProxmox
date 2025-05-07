# SMBIOS-Examples.ps1
# This script demonstrates how to use SMBIOS settings with the PSProxmox module.

# Import the PSProxmox module
Import-Module PSProxmox

# Connect to the Proxmox VE server
$credential = Get-Credential -Message "Enter your Proxmox VE credentials"
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential $credential -Realm "pam"

# Example 1: Create a VM with automatic SMBIOS settings using a specific manufacturer profile
$vm1 = New-ProxmoxVM -Connection $connection -Node "pve1" -Name "dell-server" -Memory 2048 -Cores 2 -DiskSize 32 -AutomaticSMBIOS -SMBIOSProfile "Dell"

# Example 2: Create a VM with automatic SMBIOS settings using a random manufacturer profile
$vm2 = New-ProxmoxVM -Connection $connection -Node "pve1" -Name "random-server" -Memory 2048 -Cores 2 -DiskSize 32 -AutomaticSMBIOS -SMBIOSProfile "Random"

# Example 3: Create a VM using the builder pattern with automatic SMBIOS settings
$builder = New-ProxmoxVMBuilder -Name "hp-server" -AutomaticSMBIOS -SMBIOSProfile "HP"
$builder.WithMemory(4096)
        .WithCores(2)
        .WithDisk(50, "local-lvm")
        .WithNetwork("virtio", "vmbr0")
        .WithStart($true)
$vm3 = New-ProxmoxVM -Connection $connection -Node "pve1" -Builder $builder

# Example 4: Create a VM using the builder pattern with manual SMBIOS settings
$builder = New-ProxmoxVMBuilder -Name "custom-server"
$builder.WithMemory(4096)
        .WithCores(2)
        .WithDisk(50, "local-lvm")
        .WithNetwork("virtio", "vmbr0")
        .WithSMBIOSManufacturer("Custom Manufacturer")
        .WithSMBIOSProduct("Custom Product")
        .WithSMBIOSVersion("1.0")
        .WithSMBIOSSerial("CUSTOM123")
        .WithStart($true)
$vm4 = New-ProxmoxVM -Connection $connection -Node "pve1" -Builder $builder

# Example 5: Get SMBIOS settings for an existing VM
$smbios = Get-ProxmoxVMSMBIOS -Connection $connection -Node "pve1" -VMID 100
Write-Host "Current SMBIOS settings for VM 100:"
Write-Host "Manufacturer: $($smbios.Manufacturer)"
Write-Host "Product: $($smbios.Product)"
Write-Host "Version: $($smbios.Version)"
Write-Host "Serial: $($smbios.Serial)"
Write-Host "Family: $($smbios.Family)"
Write-Host "UUID: $($smbios.UUID)"

# Example 6: Update SMBIOS settings for an existing VM using individual parameters
Set-ProxmoxVMSMBIOS -Connection $connection -Node "pve1" -VMID 100 -Manufacturer "Lenovo" -Product "ThinkSystem SR650" -Serial "LENOVO123"

# Example 7: Update SMBIOS settings for an existing VM using a manufacturer profile
$smbios = ProxmoxVMSMBIOSProfile.GetProfile("VMware")
Set-ProxmoxVMSMBIOS -Connection $connection -Node "pve1" -VMID 101 -SMBIOS $smbios

# Example 8: Create a VM with SMBIOS settings that mimic a physical server for licensing purposes
$builder = New-ProxmoxVMBuilder -Name "license-server" -AutomaticSMBIOS -SMBIOSProfile "Dell"
$builder.WithMemory(8192)
        .WithCores(4)
        .WithDisk(100, "local-lvm")
        .WithNetwork("virtio", "vmbr0")
        .WithStart($true)
$vm5 = New-ProxmoxVM -Connection $connection -Node "pve1" -Builder $builder

# Disconnect from the server when done
Disconnect-ProxmoxServer -Connection $connection
