# Advanced-MultipleVMsFromTemplate.ps1
# This script demonstrates how to create multiple VMs from a template using the PSProxmox module.

# Import the PSProxmox module
Import-Module PSProxmox

# Connect to the Proxmox VE server
$credential = Get-Credential -Message "Enter your Proxmox VE credentials"
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential $credential -Realm "pam"

# Method 1: Create multiple VMs from a template with default settings
$vms = New-ProxmoxVMFromTemplate -Node "pve1" -TemplateName "Ubuntu-Template" `
    -Prefix "web" -Count 3 -Start

# Display the VM information
$vms

# Method 2: Create multiple VMs from a template with custom settings
$vms = New-ProxmoxVMFromTemplate -Node "pve1" -TemplateName "Ubuntu-Template" `
    -Prefix "db" -Count 2 -StartIndex 1 -Memory 4096 -Cores 2 -DiskSize 50 -Start

# Display the VM information
$vms

# Method 3: Create multiple VMs from a template with IP addresses from a pool
# First, create an IP pool if it doesn't exist
if (-not (Get-ProxmoxIPPool -Name "Production")) {
    New-ProxmoxIPPool -Name "Production" -CIDR "192.168.1.0/24" -ExcludeIPs "192.168.1.1", "192.168.1.254"
}

# Create the VMs with IP addresses from the pool
$vms = New-ProxmoxVMFromTemplate -Node "pve1" -TemplateName "Ubuntu-Template" `
    -Prefix "app" -Count 5 -IPPool "Production" -Start

# Display the VM information
$vms

# Get the current connection and disconnect from the server when done
$connection = Test-ProxmoxConnection -Detailed
Disconnect-ProxmoxServer -Connection $connection
