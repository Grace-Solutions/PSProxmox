# PSProxmox

PSProxmox is a PowerShell module for managing Proxmox VE clusters. It provides a comprehensive set of cmdlets for managing virtual machines, storage, networks, users, roles, and more.

## Installation

```powershell
# Install from PowerShell Gallery
Install-Module -Name PSProxmox -Scope CurrentUser
```

## Getting Started

### Connecting to a Proxmox VE Server

```powershell
# Connect using username and password
$securePassword = ConvertTo-SecureString "password" -AsPlainText -Force
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Username "root" -Password $securePassword -Realm "pam"

# Connect using a credential object
$credential = Get-Credential
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential $credential -Realm "pam"
```

### Managing Virtual Machines

```powershell
# Get all VMs
$vms = Get-ProxmoxVM -Connection $connection

# Get a specific VM
$vm = Get-ProxmoxVM -Connection $connection -VMID 100

# Create a new VM
$vm = New-ProxmoxVM -Connection $connection -Node "pve1" -Name "test-vm" -Memory 2048 -Cores 2 -DiskSize 32 -Start

# Create a new VM using the builder pattern
$builder = New-ProxmoxVMBuilder -Name "web-server"
$builder.WithMemory(4096)
        .WithCores(2)
        .WithDisk(50, "local-lvm")
        .WithNetwork("virtio", "vmbr0")
        .WithIPConfig("192.168.1.10/24", "192.168.1.1")
        .WithStart($true)

$vm = New-ProxmoxVM -Connection $connection -Node "pve1" -Builder $builder

# Start, stop, and restart VMs
Start-ProxmoxVM -Connection $connection -Node "pve1" -VMID 100
Stop-ProxmoxVM -Connection $connection -Node "pve1" -VMID 100
Restart-ProxmoxVM -Connection $connection -Node "pve1" -VMID 100

# Remove a VM
Remove-ProxmoxVM -Connection $connection -Node "pve1" -VMID 100 -Confirm:$false
```

### Managing Templates

```powershell
# Create a template from an existing VM
$template = New-ProxmoxVMTemplate -Connection $connection -VMID 100 -Name "Ubuntu-Template" -Description "Ubuntu 20.04 Template"

# Get all templates
$templates = Get-ProxmoxVMTemplate

# Create a VM from a template
$vm = New-ProxmoxVMFromTemplate -Connection $connection -Node "pve1" -TemplateName "Ubuntu-Template" -Name "web01" -Start

# Create multiple VMs from a template
$vms = New-ProxmoxVMFromTemplate -Connection $connection -Node "pve1" -TemplateName "Ubuntu-Template" -Prefix "web" -Count 3 -Start
```

### Managing Storage

```powershell
# Get all storage
$storage = Get-ProxmoxStorage -Connection $connection

# Get storage on a specific node
$storage = Get-ProxmoxStorage -Connection $connection -Node "pve1"

# Create a new storage
$storage = New-ProxmoxStorage -Connection $connection -Name "backup" -Type "dir" -Path "/mnt/backup" -Content "backup,iso"

# Remove a storage
Remove-ProxmoxStorage -Connection $connection -Name "backup" -Confirm:$false
```

### Managing Networks

```powershell
# Get all network interfaces on a node
$networks = Get-ProxmoxNetwork -Connection $connection -Node "pve1"

# Create a new bridge interface
$network = New-ProxmoxNetwork -Connection $connection -Node "pve1" -Interface "vmbr1" -Type "bridge" -BridgePorts "eth1" -Method "static" -Address "192.168.2.1" -Netmask "255.255.255.0" -Autostart

# Remove a network interface
Remove-ProxmoxNetwork -Connection $connection -Node "pve1" -Interface "vmbr1" -Confirm:$false
```

### Managing Users and Roles

```powershell
# Get all users
$users = Get-ProxmoxUser -Connection $connection

# Create a new user
$securePassword = ConvertTo-SecureString "password" -AsPlainText -Force
$user = New-ProxmoxUser -Connection $connection -Username "john" -Realm "pam" -Password $securePassword -FirstName "John" -LastName "Doe" -Email "john.doe@example.com"

# Remove a user
Remove-ProxmoxUser -Connection $connection -UserID "john@pam" -Confirm:$false

# Get all roles
$roles = Get-ProxmoxRole -Connection $connection

# Create a new role
$role = New-ProxmoxRole -Connection $connection -RoleID "Developer" -Privileges "VM.Allocate", "VM.Config.Disk", "VM.Config.CPU", "VM.PowerMgmt"

# Remove a role
Remove-ProxmoxRole -Connection $connection -RoleID "Developer" -Confirm:$false
```

### Managing SDN

```powershell
# Get all SDN zones
$zones = Get-ProxmoxSDNZone -Connection $connection

# Create a new SDN zone
$zone = New-ProxmoxSDNZone -Connection $connection -Zone "zone1" -Type "vlan" -Bridge "vmbr0"

# Remove an SDN zone
Remove-ProxmoxSDNZone -Connection $connection -Zone "zone1" -Confirm:$false

# Get all SDN VNets
$vnets = Get-ProxmoxSDNVnet -Connection $connection

# Create a new SDN VNet
$vnet = New-ProxmoxSDNVnet -Connection $connection -VNet "vnet1" -Zone "zone1" -IPv4 "192.168.1.0/24" -Gateway "192.168.1.1"

# Remove an SDN VNet
Remove-ProxmoxSDNVnet -Connection $connection -VNet "vnet1" -Confirm:$false
```

### Managing Clusters

```powershell
# Get cluster information
$cluster = Get-ProxmoxCluster -Connection $connection

# Join a node to a cluster
$securePassword = ConvertTo-SecureString "password" -AsPlainText -Force
Join-ProxmoxCluster -Connection $connection -ClusterName "cluster1" -HostName "pve1" -Password $securePassword

# Leave a cluster
Leave-ProxmoxCluster -Connection $connection -Force

# Create a cluster backup
$backup = New-ProxmoxClusterBackup -Connection $connection -Compress -Wait

# Restore a cluster backup
Restore-ProxmoxClusterBackup -Connection $connection -BackupID "vzdump-cluster-2023_04_28-12_00_00.vma.lzo" -Force -Wait
```

### Managing IP Addresses

```powershell
# Create a new IP pool
$pool = New-ProxmoxIPPool -Name "Production" -CIDR "192.168.1.0/24" -ExcludeIPs "192.168.1.1", "192.168.1.254"

# Get all IP pools
$pools = Get-ProxmoxIPPool

# Get a specific IP pool
$pool = Get-ProxmoxIPPool -Name "Production"

# Clear an IP pool
Clear-ProxmoxIPPool -Name "Production"
```

## Disconnecting

```powershell
# Disconnect from the server
Disconnect-ProxmoxServer -Connection $connection
```

## Documentation

For detailed documentation, see the [Documentation](Documentation) directory.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
