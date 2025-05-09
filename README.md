# PSProxmox

PSProxmox is a C#-based PowerShell module for managing Proxmox VE clusters. It provides a comprehensive set of cmdlets for interacting with Proxmox VE API, featuring structured return objects, mass deployment tools, automatic IP management, and more.

## Features

- **Session Management**: Authenticate and persist sessions with Proxmox VE clusters
- **Core CRUD Operations**: Manage VMs, Storage, Network, Users, Roles, SDN, and Clusters
- **Templates**: Create and use VM deployment templates
- **Mass Creation**: Bulk create VMs with prefix/counter
- **IP Management**: CIDR parsing, subnetting, FIFO IP queue assignment
- **Structured Objects**: All outputs are typed C# classes (PowerShell-native)
- **Pipeline Support**: Cmdlets support pipeline input where appropriate

## Project Structure

- **Module/**: Contains the PowerShell module files
- **PSProxmox/**: Contains the C# source code for the module
- **Documentation/**: Contains detailed documentation and examples
- **Scripts/**: Contains build and installation scripts
- **Release/**: Contains built releases (created by build script)

## Installation

### From PowerShell Gallery (Recommended)

```powershell
# Install from PowerShell Gallery
Install-Module -Name PSProxmox -Scope CurrentUser
```

### Manual Installation

```powershell
# Clone the repository
git clone https://github.com/Grace-Solutions/PSProxmox.git
cd PSProxmox

# Build the module
.\Scripts\build.ps1

# Install the module
.\Scripts\Install-PSProxmox.ps1
```

## Getting Started

### Connecting to a Proxmox VE Server

```powershell
# Connect using username and password
$securePassword = ConvertTo-SecureString "password" -AsPlainText -Force
Connect-ProxmoxServer -Server "proxmox.example.com" -Username "root" -Password $securePassword -Realm "pam"

# Connect using a credential object
$credential = Get-Credential
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential $credential -Realm "pam"

# Test the connection
Test-ProxmoxConnection -Detailed
```

### Managing Virtual Machines

```powershell
# Get all VMs (uses the default connection automatically)
$vms = Get-ProxmoxVM

# Get a specific VM
$vm = Get-ProxmoxVM -VMID 100

# Create a new VM
$vm = New-ProxmoxVM -Node "pve1" -Name "test-vm" -Memory 2048 -Cores 2 -DiskSize 32 -Start

# Create a new VM using the builder pattern
$builder = New-ProxmoxVMBuilder -Name "web-server"
$builder.WithMemory(4096)
        .WithCores(2)
        .WithDisk(50, "local-lvm")
        .WithNetwork("virtio", "vmbr0")
        .WithIPConfig("192.168.1.10/24", "192.168.1.1")
        .WithStart($true)

$vm = New-ProxmoxVM -Node "pve1" -Builder $builder

# Start, stop, and restart VMs
Start-ProxmoxVM -Node "pve1" -VMID 100
Stop-ProxmoxVM -Node "pve1" -VMID 100
Restart-ProxmoxVM -Node "pve1" -VMID 100

# Remove a VM
Remove-ProxmoxVM -Node "pve1" -VMID 100 -Confirm:$false
```

### Managing Templates

```powershell
# Create a template from an existing VM
$template = New-ProxmoxVMTemplate -VMID 100 -Name "Ubuntu-Template" -Description "Ubuntu 20.04 Template"

# Get all templates
$templates = Get-ProxmoxVMTemplate

# Create a VM from a template
$vm = New-ProxmoxVMFromTemplate -Node "pve1" -TemplateName "Ubuntu-Template" -Name "web01" -Start

# Create multiple VMs from a template
$vms = New-ProxmoxVMFromTemplate -Node "pve1" -TemplateName "Ubuntu-Template" -Prefix "web" -Count 3 -Start
```

### Managing Storage

```powershell
# Get all storage
$storage = Get-ProxmoxStorage

# Get storage on a specific node
$storage = Get-ProxmoxStorage -Node "pve1"

# Create a new storage
$storage = New-ProxmoxStorage -Name "backup" -Type "dir" -Path "/mnt/backup" -Content "backup,iso"

# Remove a storage
Remove-ProxmoxStorage -Name "backup" -Confirm:$false
```

### Managing Networks

```powershell
# Get all network interfaces on a node
$networks = Get-ProxmoxNetwork -Node "pve1"

# Create a new bridge interface
$network = New-ProxmoxNetwork -Node "pve1" -Interface "vmbr1" -Type "bridge" -BridgePorts "eth1" -Method "static" -Address "192.168.2.1" -Netmask "255.255.255.0" -Autostart

# Remove a network interface
Remove-ProxmoxNetwork -Node "pve1" -Interface "vmbr1" -Confirm:$false
```

### Managing Users and Roles

```powershell
# Get all users
$users = Get-ProxmoxUser

# Create a new user
$securePassword = ConvertTo-SecureString "password" -AsPlainText -Force
$user = New-ProxmoxUser -Username "john" -Realm "pam" -Password $securePassword -FirstName "John" -LastName "Doe" -Email "john.doe@example.com"

# Remove a user
Remove-ProxmoxUser -UserID "john@pam" -Confirm:$false

# Get all roles
$roles = Get-ProxmoxRole

# Create a new role
$role = New-ProxmoxRole -RoleID "Developer" -Privileges "VM.Allocate", "VM.Config.Disk", "VM.Config.CPU", "VM.PowerMgmt"

# Remove a role
Remove-ProxmoxRole -RoleID "Developer" -Confirm:$false
```

### Managing SDN

```powershell
# Get all SDN zones
$zones = Get-ProxmoxSDNZone

# Create a new SDN zone
$zone = New-ProxmoxSDNZone -Zone "zone1" -Type "vlan" -Bridge "vmbr0"

# Remove an SDN zone
Remove-ProxmoxSDNZone -Zone "zone1" -Confirm:$false

# Get all SDN VNets
$vnets = Get-ProxmoxSDNVnet

# Create a new SDN VNet
$vnet = New-ProxmoxSDNVnet -VNet "vnet1" -Zone "zone1" -IPv4 "192.168.1.0/24" -Gateway "192.168.1.1"

# Remove an SDN VNet
Remove-ProxmoxSDNVnet -VNet "vnet1" -Confirm:$false
```

### Managing Clusters

```powershell
# Get cluster information
$cluster = Get-ProxmoxCluster

# Join a node to a cluster
$securePassword = ConvertTo-SecureString "password" -AsPlainText -Force
Join-ProxmoxCluster -ClusterName "cluster1" -HostName "pve1" -Password $securePassword

# Leave a cluster
Leave-ProxmoxCluster -Force

# Create a cluster backup
$backup = New-ProxmoxClusterBackup -Compress -Wait

# Restore a cluster backup
Restore-ProxmoxClusterBackup -BackupID "vzdump-cluster-2023_04_28-12_00_00.vma.lzo" -Force -Wait
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
# Get the current connection and disconnect from the server
$connection = Test-ProxmoxConnection -Detailed
Disconnect-ProxmoxServer -Connection $connection
```

## Development

### Building the Module

To build the module from source:

```powershell
# Run the build script
.\Scripts\build.ps1
```

This will:
1. Compile the C# code
2. Create a versioned release in the Release folder
3. Update the Module folder with the latest build
4. Create a ZIP package for distribution

### Project Organization

- **C# Source Code**: All C# source code is in the PSProxmox folder
- **PowerShell Module**: The PowerShell module files are in the Module folder
- **Documentation**: Comprehensive documentation is in the Documentation folder
- **Scripts**: Build and installation scripts are in the Scripts folder

## Documentation

For detailed documentation, see the [Documentation](Documentation) directory. The documentation includes:

- [Cmdlet Reference](Documentation/cmdlets/README.md)
- [Examples](Documentation/examples/README.md)
- [Guides](Documentation/guides/README.md)

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
