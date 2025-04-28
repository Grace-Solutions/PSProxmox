# PSProxmox Documentation

PSProxmox is a C#-based PowerShell module for managing Proxmox VE clusters. It provides a comprehensive set of cmdlets for interacting with Proxmox VE API, featuring structured return objects, mass deployment tools, automatic IP management, and more.

## Features

- **Session Management**: Authenticate and persist sessions with Proxmox VE clusters
- **Core CRUD Operations**: Manage VMs, Storage, Network, Users, Roles, SDN, and Clusters
- **Templates**: Create and use VM deployment templates
- **Mass Creation**: Bulk create VMs with prefix/counter
- **IP Management**: CIDR parsing, subnetting, FIFO IP queue assignment
- **Structured Objects**: All outputs are typed C# classes (PowerShell-native)
- **Pipeline Support**: Cmdlets support pipeline input where appropriate

## Getting Started

- [Installation Guide](guides/Installation.md)
- [Connection Guide](guides/Connection.md)
- [Authentication Methods](guides/Authentication.md)

## Documentation

- [Cmdlet Reference](cmdlets/README.md) - Detailed documentation for all cmdlets
- [Examples](examples/README.md) - Example scripts for common tasks
- [Guides](guides/README.md) - Comprehensive guides for various topics

## Requirements

- PowerShell 5.1 or PowerShell 7+
- .NET Framework 4.7.2 or .NET Core 2.0+

## Quick Start

```powershell
# Install from PowerShell Gallery
Install-Module -Name PSProxmox -Scope CurrentUser

# Import the module
Import-Module PSProxmox

# Connect to a Proxmox VE server
$credential = Get-Credential
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential $credential -Realm "pam"

# Get all VMs
$vms = Get-ProxmoxVM -Connection $connection

# Create a new VM
$vm = New-ProxmoxVM -Connection $connection -Node "pve1" -Name "test-vm" -Memory 2048 -Cores 2 -DiskSize 32 -Start

# Disconnect when done
Disconnect-ProxmoxServer -Connection $connection
```

## Using the Documentation

For detailed documentation on each cmdlet, you can:

1. Use the built-in PowerShell help:
   ```powershell
   Get-Help Connect-ProxmoxServer -Full
   ```

2. Browse the [Cmdlet Reference](cmdlets/README.md) for detailed documentation on all cmdlets.

3. Check the [Examples](examples/README.md) for example scripts for common tasks.

4. Read the [Guides](guides/README.md) for comprehensive guides on various topics.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.
