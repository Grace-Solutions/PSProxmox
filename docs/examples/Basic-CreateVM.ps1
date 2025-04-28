# Basic-CreateVM.ps1
# This script demonstrates how to create a new VM using the PSProxmox module.

# Import the PSProxmox module
Import-Module PSProxmox

# Connect to the Proxmox VE server
$credential = Get-Credential -Message "Enter your Proxmox VE credentials"
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential $credential -Realm "pam"

# Method 1: Create a basic VM
$vm = New-ProxmoxVM -Connection $connection -Node "pve1" -Name "test-vm" -Memory 2048 -Cores 2 -DiskSize 32 -Start

# Display the VM information
$vm

# Method 2: Create a VM with more options
$vm = New-ProxmoxVM -Connection $connection -Node "pve1" -Name "web-server" `
    -Memory 4096 -Cores 2 -Sockets 1 -DiskSize 50 -Storage "local-lvm" `
    -OSType "l26" -NetworkModel "virtio" -NetworkBridge "vmbr0" `
    -Description "Web server for testing" -Start

# Display the VM information
$vm

# Method 3: Create a VM with a specific ID
$vm = New-ProxmoxVM -Connection $connection -Node "pve1" -Name "db-server" -VMID 200 `
    -Memory 8192 -Cores 4 -DiskSize 100 -Storage "local-lvm" -Start

# Display the VM information
$vm

# Disconnect from the server when done
Disconnect-ProxmoxServer -Connection $connection
