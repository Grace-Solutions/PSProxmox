# Advanced-MultipleVMsWithPaddedCounters.ps1
# This script demonstrates how to create multiple VMs with padded counters and automatic SMBIOS settings.

# Import the PSProxmox module
Import-Module PSProxmox

# Connect to the Proxmox VE server
$credential = Get-Credential -Message "Enter your Proxmox VE credentials"
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential $credential -Realm "pam"

# Method 1: Create multiple VMs with padded counters
$vms = New-ProxmoxVMFromTemplate -Node "pve1" -TemplateName "Ubuntu-Template" `
    -Prefix "web-" -Count 3 -CounterDigits 3 -Start

# Display the VM information
Write-Host "Created VMs with padded counters:"
$vms | Select-Object Name, VMID, Status | Format-Table

# Method 2: Create multiple VMs with padded counters and custom starting index
$vms = New-ProxmoxVMFromTemplate -Node "pve1" -TemplateName "Ubuntu-Template" `
    -Prefix "db-" -Count 2 -StartIndex 10 -CounterDigits 4 -Memory 4096 -Cores 2 -Start

# Display the VM information
Write-Host "Created VMs with padded counters and custom starting index:"
$vms | Select-Object Name, VMID, Status | Format-Table

# Method 3: Create multiple VMs with automatic SMBIOS settings
$vms = New-ProxmoxVMFromTemplate -Node "pve1" -TemplateName "Windows-Template" `
    -Prefix "win-" -Count 3 -CounterDigits 2 -AutomaticSMBIOS -SMBIOSProfile "Dell" -Start

# Display the VM information
Write-Host "Created VMs with Dell SMBIOS settings:"
$vms | Select-Object Name, VMID, Status | Format-Table

# Method 4: Create multiple VMs with different SMBIOS profiles
$manufacturers = @("Dell", "HP", "Lenovo", "Microsoft")
$count = $manufacturers.Count

$vms = New-ProxmoxVMFromTemplate -Node "pve1" -TemplateName "Windows-Template" `
    -Prefix "test-" -Count $count -CounterDigits 2 -Start

# Update each VM with a different SMBIOS profile
for ($i = 0; $i -lt $count; $i++) {
    $profile = $manufacturers[$i]
    $vmid = $vms[$i].VMID

    Write-Host "Setting $profile SMBIOS profile for VM $vmid..."
    Set-ProxmoxVMSMBIOS -Node "pve1" -VMID $vmid -UseProfile -Profile $profile
}

# Display the VM information
Write-Host "Created VMs with different SMBIOS profiles:"
$vms | Select-Object Name, VMID, Status | Format-Table

# Get the current connection and disconnect from the server when done
$connection = Test-ProxmoxConnection -Detailed
Disconnect-ProxmoxServer -Connection $connection
