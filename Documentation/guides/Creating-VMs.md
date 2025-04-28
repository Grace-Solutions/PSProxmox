# Creating VMs

This guide explains how to create virtual machines using the PSProxmox module.

## Prerequisites

- PSProxmox module installed
- Connection to a Proxmox VE server

## Basic VM Creation

The simplest way to create a VM is to use the `New-ProxmoxVM` cmdlet with basic parameters:

```powershell
# Connect to the Proxmox VE server
$credential = Get-Credential
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential $credential -Realm "pam"

# Create a basic VM
$vm = New-ProxmoxVM -Connection $connection -Node "pve1" -Name "test-vm" -Memory 2048 -Cores 2 -DiskSize 32 -Start
```

This creates a VM with the following specifications:
- Name: test-vm
- Memory: 2 GB
- CPU: 2 cores
- Disk: 32 GB
- The VM will be started after creation

## Advanced VM Creation

For more control over the VM configuration, you can specify additional parameters:

```powershell
$vm = New-ProxmoxVM -Connection $connection -Node "pve1" -Name "web-server" `
    -Memory 4096 -Cores 2 -Sockets 1 -DiskSize 50 -Storage "local-lvm" `
    -OSType "l26" -NetworkModel "virtio" -NetworkBridge "vmbr0" `
    -Description "Web server for testing" -Start
```

This creates a VM with more specific configuration:
- Name: web-server
- Memory: 4 GB
- CPU: 2 cores, 1 socket
- Disk: 50 GB on local-lvm storage
- OS Type: Linux 2.6+ kernel
- Network: virtio model on vmbr0 bridge
- Description: "Web server for testing"
- The VM will be started after creation

## Using the VM Builder Pattern

For complex VM configurations, you can use the VM builder pattern:

```powershell
# Create a VM builder
$builder = New-ProxmoxVMBuilder -Name "db-server"

# Configure the VM using the builder pattern
$builder.WithMemory(8192)                                # 8 GB of memory
        .WithCores(4)                                    # 4 CPU cores
        .WithSockets(2)                                  # 2 CPU sockets
        .WithDisk(100, "local-lvm")                      # 100 GB primary disk on local-lvm storage
        .WithAdditionalDisk(200, "local-lvm")            # 200 GB additional disk on local-lvm storage
        .WithNetwork("virtio", "vmbr0")                  # virtio network on vmbr0 bridge
        .WithAdditionalNetwork("virtio", "vmbr1")        # Additional network interface on vmbr1
        .WithIPConfig("192.168.1.20/24", "192.168.1.1")  # Static IP configuration for first interface
        .WithCPUType("host")                             # CPU type
        .WithVGA("std")                                  # VGA type
        .WithBoot("order=scsi0;net0")                    # Boot order
        .WithDescription("Database server for production") # Description
        .WithStart($true)                                # Start the VM after creation

# Create the VM using the builder
$vm = New-ProxmoxVM -Connection $connection -Node "pve1" -Builder $builder
```

The builder pattern allows you to:
- Chain configuration methods together
- Configure complex settings like multiple disks and network interfaces
- Set advanced parameters like CPU type, VGA type, and boot order

## Creating VMs from Templates

If you have VM templates, you can create VMs from them using the `New-ProxmoxVMFromTemplate` cmdlet:

```powershell
# Create a VM from a template
$vm = New-ProxmoxVMFromTemplate -Connection $connection -Node "pve1" -TemplateName "Ubuntu-Template" -Name "web01" -Start
```

You can also override template settings:

```powershell
# Create a VM from a template with custom settings
$vm = New-ProxmoxVMFromTemplate -Connection $connection -Node "pve1" -TemplateName "Ubuntu-Template" `
    -Name "web01" -Memory 4096 -Cores 2 -DiskSize 50 -Start
```

## Creating Multiple VMs from Templates

You can create multiple VMs from a template in one operation:

```powershell
# Create multiple VMs from a template
$vms = New-ProxmoxVMFromTemplate -Connection $connection -Node "pve1" -TemplateName "Ubuntu-Template" `
    -Prefix "web" -Count 3 -Start
```

This creates three VMs named "web1", "web2", and "web3".

You can also specify a starting index:

```powershell
# Create multiple VMs from a template with a custom starting index
$vms = New-ProxmoxVMFromTemplate -Connection $connection -Node "pve1" -TemplateName "Ubuntu-Template" `
    -Prefix "web" -Count 3 -StartIndex 10 -Start
```

This creates three VMs named "web10", "web11", and "web12".

## Creating VMs with IP Addresses from a Pool

If you have IP pools configured, you can assign IP addresses from them:

```powershell
# Create a VM with an IP address from a pool
$vm = New-ProxmoxVMFromTemplate -Connection $connection -Node "pve1" -TemplateName "Ubuntu-Template" `
    -Name "web01" -IPPool "Production" -Start
```

This assigns an IP address from the "Production" pool to the VM.

## Best Practices

- Use descriptive names for your VMs
- Use templates for consistent VM configurations
- Use the builder pattern for complex VM configurations
- Document your VM configurations
- Use IP pools for IP address management
- Test your VM configurations before deploying to production

## Next Steps

Now that you know how to create VMs, you can proceed to the [Managing VMs](Managing-VMs.md) guide to learn how to manage them.
