# Advanced-VMBuilder.ps1
# This script demonstrates how to create a VM using the builder pattern with the PSProxmox module.

# Import the PSProxmox module
Import-Module PSProxmox

# Connect to the Proxmox VE server
$credential = Get-Credential -Message "Enter your Proxmox VE credentials"
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential $credential -Realm "pam"

# Create a VM builder
$builder = New-ProxmoxVMBuilder -Name "web-server"

# Configure the VM using the builder pattern
$builder.WithMemory(4096)                                # 4 GB of memory
        .WithCores(2)                                    # 2 CPU cores
        .WithSockets(1)                                  # 1 CPU socket
        .WithDisk(50, "local-lvm")                       # 50 GB disk on local-lvm storage
        .WithNetwork("virtio", "vmbr0")                  # virtio network on vmbr0 bridge
        .WithIPConfig("192.168.1.10/24", "192.168.1.1")  # Static IP configuration
        .WithDescription("Web server for production")     # Description
        .WithStart($true)                                # Start the VM after creation

# Create the VM using the builder (uses the default connection automatically)
$vm = New-ProxmoxVM -Node "pve1" -Builder $builder

# Display the VM information
$vm

# Create another VM with a more complex configuration
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
$vm = New-ProxmoxVM -Node "pve1" -Builder $builder

# Display the VM information
$vm

# Get the current connection and disconnect from the server when done
$connection = Test-ProxmoxConnection -Detailed
Disconnect-ProxmoxServer -Connection $connection
