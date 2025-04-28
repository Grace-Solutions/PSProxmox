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

## Requirements

- PowerShell 5.1 or PowerShell 7+
- .NET Framework 4.7.2 or .NET Core 2.0+

## Installation

```powershell
# Install from PSGallery (when published)
Install-Module -Name PSProxmox

# Import the module
Import-Module PSProxmox
```

## Quick Start

```powershell
# Connect to a Proxmox server
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)

# Get all VMs
$vms = Get-ProxmoxVM -Connection $connection

# Create a new VM
$newVM = New-ProxmoxVM -Connection $connection -Name "test-vm" -Memory 2048 -Cores 2 -DiskSize 32

# Start a VM
Start-ProxmoxVM -Connection $connection -VMID $newVM.VMID

# Disconnect from the server
Disconnect-ProxmoxServer -Connection $connection
```

## Documentation

For detailed documentation on each cmdlet, use the built-in PowerShell help:

```powershell
Get-Help Connect-ProxmoxServer -Full
```

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
