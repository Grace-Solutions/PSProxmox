# PSProxmox

PSProxmox is a C#-based PowerShell module for managing Proxmox VE clusters. It provides a comprehensive set of cmdlets for interacting with Proxmox VE API, featuring structured return objects, mass deployment tools, automatic IP management, and more.

## Features

- **Session Management**: Authenticate and persist sessions with Proxmox VE clusters
- **Core CRUD Operations**: Manage VMs, LXC Containers, Storage, Network, Users, Roles, SDN, and Clusters
- **VM Guest Agent Support**: Comprehensive guest agent data retrieval with network interface information
- **Templates**: Create and use VM deployment templates
- **Cloud Images**: Download, customize, and create templates from cloud images
- **Cloud-Init Support**: Configure VMs with Cloud-Init for easy customization
- **LXC Containers**: Create and manage Linux Containers with full CRUD operations
- **TurnKey Templates**: Download and use TurnKey Linux templates for LXC containers
- **Mass Creation**: Bulk create VMs with prefix/counter
- **IP Management**: CIDR parsing, subnetting, FIFO IP queue assignment
- **Structured Objects**: All outputs are typed C# classes (PowerShell-native)
- **Pipeline Support**: Cmdlets support pipeline input where appropriate
- **Clean Codebase**: 100% compilation success with zero errors and warnings

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

### Working with VM Guest Agent

```powershell
# Get VM with guest agent information
$vm = Get-ProxmoxVM -VMID 100

# Check if guest agent is available and running
if ($vm.GuestAgent -and $vm.GuestAgent.Status -eq "running") {
    Write-Host "Guest Agent is running"

    # Display network interfaces from guest agent
    foreach ($interface in $vm.GuestAgent.NetIf) {
        Write-Host "Interface: $($interface.Name)"
        Write-Host "  IPv4 Addresses: $($interface.IPv4Addresses -join ', ')"
        Write-Host "  IPv6 Addresses: $($interface.IPv6Addresses -join ', ')"
        Write-Host "  MAC Address: $($interface.MacAddress)"
    }
} else {
    Write-Host "Guest Agent not available or not running"
}

# Get all VMs with active guest agents
$vmsWithGuestAgent = Get-ProxmoxVM | Where-Object {
    $_.GuestAgent -and $_.GuestAgent.Status -eq "running"
}

# Find VMs by IP address using guest agent data
$searchIP = "192.168.1.100"
$foundVMs = $vmsWithGuestAgent | Where-Object {
    $vm = $_
    $found = $false

    if ($vm.GuestAgent.NetIf) {
        foreach ($interface in $vm.GuestAgent.NetIf) {
            if ($interface.IPv4Addresses -contains $searchIP -or
                $interface.IPv6Addresses -contains $searchIP) {
                $found = $true
                break
            }
        }
    }

    return $found
}
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

### Working with Cloud Images

```powershell
# List available cloud images
Get-ProxmoxCloudImage -Distribution "ubuntu" -Release "22.04"

# Download a cloud image
$imagePath = Save-ProxmoxCloudImage -Distribution "ubuntu" -Release "22.04" -Variant "server"

# Customize the cloud image (resize to 20GB)
$customizedImagePath = Invoke-ProxmoxCloudImageCustomization -ImagePath $imagePath -Resize 20 -ConvertTo "qcow2"

# Create a template from the cloud image
$template = New-ProxmoxCloudImageTemplate -Node "pve1" -Name "ubuntu-22.04" -ImagePath $customizedImagePath -Storage "local-lvm" -Memory 2048 -Cores 2 -DiskSize 20

# Alternatively, download and create a template in one step
$template = New-ProxmoxCloudImageTemplate -Node "pve1" -Name "ubuntu-22.04" -Distribution "ubuntu" -Release "22.04" -Storage "local-lvm" -Memory 2048 -Cores 2 -DiskSize 20

# Configure Cloud-Init for a VM
$password = ConvertTo-SecureString "SecurePassword123!" -AsPlainText -Force
$sshKey = Get-Content -Path "~/.ssh/id_rsa.pub"
Set-ProxmoxVMCloudInit -Node "pve1" -VMID 100 -Username "admin" -Password $password -SSHKey $sshKey -IPConfig "dhcp" -DNS "8.8.8.8,8.8.4.4"

# Create a VM from the template with static IP
$vm = New-ProxmoxVMFromTemplate -Node "pve1" -TemplateName "ubuntu-22.04" -Name "web01"
Set-ProxmoxVMCloudInit -Node "pve1" -VMID $vm.VMID -Username "admin" -Password $password -IPConfig "ip=192.168.1.100/24,gw=192.168.1.1"
Start-ProxmoxVM -Node "pve1" -VMID $vm.VMID
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

### Managing LXC Containers

```powershell
# Get all LXC containers
$containers = Get-ProxmoxContainer

# Get a specific container
$container = Get-ProxmoxContainer -CTID 100

# Get containers by name pattern (supports wildcards and regex)
$webContainers = Get-ProxmoxContainer -Name "web*"
$dbContainers = Get-ProxmoxContainer -Name "^db\d+$"

# Create a new container
$container = New-ProxmoxContainer -Node "pve1" -Name "web-container" -OSTemplate "local:vztmpl/ubuntu-20.04-standard_20.04-1_amd64.tar.gz" -Storage "local-lvm" -Memory 512 -Cores 1 -DiskSize 8 -Unprivileged -Start

# Create a container using a builder
$builder = New-ProxmoxContainerBuilder -Name "db-container"
$builder.WithOSTemplate("local:vztmpl/ubuntu-20.04-standard_20.04-1_amd64.tar.gz")
        .WithStorage("local-lvm")
        .WithMemory(1024)
        .WithCores(2)
        .WithDiskSize(16)
        .WithUnprivileged($true)
        .WithStart($true)

$container = New-ProxmoxContainer -Node "pve1" -Builder $builder

# Start, stop, and restart containers
Start-ProxmoxContainer -Node "pve1" -CTID 100 -Wait
Stop-ProxmoxContainer -Node "pve1" -CTID 100 -Wait
Restart-ProxmoxContainer -Node "pve1" -CTID 100 -Wait

# Remove a container
Remove-ProxmoxContainer -Node "pve1" -CTID 100 -Confirm:$false
```

### Working with TurnKey Linux Templates

```powershell
# List available TurnKey templates
$templates = Get-ProxmoxTurnKeyTemplate

# Get TurnKey templates by name pattern
$wordpressTemplates = Get-ProxmoxTurnKeyTemplate -Name "wordpress*"

# Download a TurnKey template
$templatePath = Save-ProxmoxTurnKeyTemplate -Node "pve1" -Name "wordpress" -Storage "local"

# Create a container from a TurnKey template
$container = New-ProxmoxContainerFromTurnKey -Node "pve1" -Name "wordpress" -Template "wordpress" -Storage "local-lvm" -Memory 512 -Cores 1 -DiskSize 8 -Start
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
  - [Installation Guide](Documentation/guides/Installation.md)
  - [Cloud Image Templates Guide](Documentation/guides/CloudImageTemplates.md)
  - [LXC Containers Guide](Documentation/guides/LXCContainers.md)

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
