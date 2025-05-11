# LXC Container Management Guide

This guide covers how to use PSProxmox to manage LXC containers on a Proxmox VE server.

## Overview

Proxmox VE supports Linux Containers (LXC) as a lightweight alternative to full virtual machines. LXC containers provide an isolated environment for applications with minimal overhead compared to VMs.

PSProxmox provides cmdlets for creating, managing, and removing LXC containers, as well as working with TurnKey Linux templates.

## Container Management Cmdlets

PSProxmox includes the following cmdlets for managing LXC containers:

- `Get-ProxmoxContainer`: List all LXC containers or get details for a specific container
- `New-ProxmoxContainer`: Create a new LXC container
- `New-ProxmoxContainerBuilder`: Create a builder for configuring container parameters
- `Remove-ProxmoxContainer`: Delete an LXC container
- `Start-ProxmoxContainer`: Start an LXC container
- `Stop-ProxmoxContainer`: Stop an LXC container
- `Restart-ProxmoxContainer`: Restart an LXC container

## TurnKey Template Management Cmdlets

PSProxmox also includes cmdlets for working with TurnKey Linux templates:

- `Get-ProxmoxTurnKeyTemplate`: List available TurnKey templates
- `Save-ProxmoxTurnKeyTemplate`: Download a TurnKey template to a Proxmox storage
- `New-ProxmoxContainerFromTurnKey`: Create a new LXC container from a TurnKey template

## Basic Container Operations

### Listing Containers

To list all containers on a Proxmox server:

```powershell
Get-ProxmoxContainer
```

To get a specific container by ID:

```powershell
Get-ProxmoxContainer -CTID 100
```

To get containers on a specific node:

```powershell
Get-ProxmoxContainer -Node "pve1"
```

To get containers by name pattern (supports wildcards and regex):

```powershell
# Using wildcards
Get-ProxmoxContainer -Name "web*"

# Using regex
Get-ProxmoxContainer -Name "^web\d+$"
```

### Creating Containers

To create a new container using parameters:

```powershell
New-ProxmoxContainer -Node "pve1" -Name "web-container" -OSTemplate "local:vztmpl/ubuntu-20.04-standard_20.04-1_amd64.tar.gz" -Storage "local-lvm" -Memory 512 -Swap 512 -Cores 1 -DiskSize 8 -Unprivileged -StartOnBoot -Start
```

To create a new container using a builder (for more complex configurations):

```powershell
$builder = New-ProxmoxContainerBuilder -Name "db-container"
$builder.WithOSTemplate("local:vztmpl/ubuntu-20.04-standard_20.04-1_amd64.tar.gz")
        .WithStorage("local-lvm")
        .WithMemory(1024)
        .WithSwap(1024)
        .WithCores(2)
        .WithDiskSize(16)
        .WithUnprivileged($true)
        .WithStartOnBoot($true)
        .WithStart($true)
        .WithDescription("Database container")

New-ProxmoxContainer -Node "pve1" -Builder $builder
```

### Managing Container State

To start a container:

```powershell
Start-ProxmoxContainer -Node "pve1" -CTID 100 -Wait
```

To stop a container:

```powershell
Stop-ProxmoxContainer -Node "pve1" -CTID 100 -Wait
```

To restart a container:

```powershell
Restart-ProxmoxContainer -Node "pve1" -CTID 100 -Wait
```

### Removing Containers

To remove a container:

```powershell
Remove-ProxmoxContainer -Node "pve1" -CTID 100 -Confirm:$false
```

## Working with TurnKey Linux Templates

TurnKey Linux provides pre-built appliances that can be used as templates for LXC containers.

### Listing TurnKey Templates

To list all available TurnKey templates:

```powershell
Get-ProxmoxTurnKeyTemplate
```

To list templates by name pattern:

```powershell
Get-ProxmoxTurnKeyTemplate -Name "wordpress*"
```

To include templates that have already been downloaded:

```powershell
Get-ProxmoxTurnKeyTemplate -IncludeDownloaded
```

### Downloading TurnKey Templates

To download a TurnKey template to a storage:

```powershell
Save-ProxmoxTurnKeyTemplate -Node "pve1" -Name "wordpress" -Storage "local"
```

### Creating Containers from TurnKey Templates

To create a container from a TurnKey template:

```powershell
New-ProxmoxContainerFromTurnKey -Node "pve1" -Name "wordpress" -Template "wordpress" -Storage "local-lvm" -Memory 512 -Cores 1 -DiskSize 8 -Start
```

## Best Practices

1. **Use Unprivileged Containers**: For better security, create unprivileged containers when possible.
2. **Resource Allocation**: Allocate appropriate resources (memory, CPU, disk) based on the container's purpose.
3. **Storage Selection**: Choose the appropriate storage type for your containers (local-lvm is often a good choice for performance).
4. **Naming Convention**: Use a consistent naming convention for your containers to make management easier.
5. **Template Management**: Download and maintain templates on a central storage for easy access from all nodes.

## Examples

See the [LXC Container Management Examples](../examples/LXC-Container-Management.ps1) for more detailed examples.

## Related Documentation

- [Proxmox VE LXC Documentation](https://pve.proxmox.com/wiki/Linux_Container)
- [TurnKey Linux](https://www.turnkeylinux.org/)
