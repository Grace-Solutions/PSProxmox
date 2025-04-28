# New-ProxmoxVM

Creates a new virtual machine in Proxmox VE.

## Syntax

```powershell
New-ProxmoxVM
   -Connection <ProxmoxConnection>
   -Node <String>
   -Name <String>
   [-VMID <Int32>]
   [-Memory <Int32>]
   [-Cores <Int32>]
   [-Sockets <Int32>]
   [-DiskSize <Int32>]
   [-Storage <String>]
   [-OSType <String>]
   [-NetworkModel <String>]
   [-NetworkBridge <String>]
   [-Description <String>]
   [-Start]
   [<CommonParameters>]
```

```powershell
New-ProxmoxVM
   -Connection <ProxmoxConnection>
   -Node <String>
   -Builder <ProxmoxVMBuilder>
   [<CommonParameters>]
```

## Description

The `New-ProxmoxVM` cmdlet creates a new virtual machine in Proxmox VE. You can specify the VM parameters directly or use a VM builder object.

## Parameters

### -Connection

The connection to the Proxmox VE server.

```yaml
Type: ProxmoxConnection
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Node

The node to create the VM on.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Name

The name of the VM to create.

```yaml
Type: String
Parameter Sets: Direct
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -VMID

The VM ID. If not specified, the next available ID will be used.

```yaml
Type: Int32
Parameter Sets: Direct
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Memory

The amount of memory in MB.

```yaml
Type: Int32
Parameter Sets: Direct
Aliases:

Required: False
Position: Named
Default value: 512
Accept pipeline input: False
Accept wildcard characters: False
```

### -Cores

The number of CPU cores.

```yaml
Type: Int32
Parameter Sets: Direct
Aliases:

Required: False
Position: Named
Default value: 1
Accept pipeline input: False
Accept wildcard characters: False
```

### -Sockets

The number of CPU sockets.

```yaml
Type: Int32
Parameter Sets: Direct
Aliases:

Required: False
Position: Named
Default value: 1
Accept pipeline input: False
Accept wildcard characters: False
```

### -DiskSize

The disk size in GB.

```yaml
Type: Int32
Parameter Sets: Direct
Aliases:

Required: False
Position: Named
Default value: 8
Accept pipeline input: False
Accept wildcard characters: False
```

### -Storage

The storage location for the disk.

```yaml
Type: String
Parameter Sets: Direct
Aliases:

Required: False
Position: Named
Default value: local-lvm
Accept pipeline input: False
Accept wildcard characters: False
```

### -OSType

The operating system type.

```yaml
Type: String
Parameter Sets: Direct
Aliases:

Required: False
Position: Named
Default value: l26
Accept pipeline input: False
Accept wildcard characters: False
```

### -NetworkModel

The network interface model.

```yaml
Type: String
Parameter Sets: Direct
Aliases:

Required: False
Position: Named
Default value: virtio
Accept pipeline input: False
Accept wildcard characters: False
```

### -NetworkBridge

The network bridge.

```yaml
Type: String
Parameter Sets: Direct
Aliases:

Required: False
Position: Named
Default value: vmbr0
Accept pipeline input: False
Accept wildcard characters: False
```

### -Description

The description of the VM.

```yaml
Type: String
Parameter Sets: Direct
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Start

Whether to start the VM after creation.

```yaml
Type: SwitchParameter
Parameter Sets: Direct
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -Builder

A VM builder object containing the VM configuration.

```yaml
Type: ProxmoxVMBuilder
Parameter Sets: Builder
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## Inputs

### None

## Outputs

### PSProxmox.Models.ProxmoxVM

## Notes

- This cmdlet requires a connection to a Proxmox VE server. Use `Connect-ProxmoxServer` to establish a connection.
- If the `-VMID` parameter is not specified, the next available ID will be used.
- If the `-Start` parameter is specified, the VM will be started after creation.
- You can use the `New-ProxmoxVMBuilder` cmdlet to create a VM builder object for more complex VM configurations.

## Examples

### Example 1: Create a basic VM

```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$vm = New-ProxmoxVM -Connection $connection -Node "pve1" -Name "test-vm" -Memory 2048 -Cores 2 -DiskSize 32 -Start
```

This example creates a new VM with 2 GB of memory, 2 CPU cores, and a 32 GB disk, and starts it.

### Example 2: Create a VM using a builder

```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$builder = New-ProxmoxVMBuilder -Name "web-server"
$builder.WithMemory(4096)
        .WithCores(2)
        .WithDisk(50, "local-lvm")
        .WithNetwork("virtio", "vmbr0")
        .WithIPConfig("192.168.1.10/24", "192.168.1.1")
        .WithStart($true)

$vm = New-ProxmoxVM -Connection $connection -Node "pve1" -Builder $builder
```

This example creates a new VM using a builder object with 4 GB of memory, 2 CPU cores, a 50 GB disk, and a static IP address, and starts it.

### Example 3: Create a VM with a specific ID

```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$vm = New-ProxmoxVM -Connection $connection -Node "pve1" -Name "test-vm" -VMID 100 -Memory 2048 -Cores 2 -DiskSize 32
```

This example creates a new VM with ID 100.

### Example 4: Create a VM with a description

```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$vm = New-ProxmoxVM -Connection $connection -Node "pve1" -Name "test-vm" -Memory 2048 -Cores 2 -DiskSize 32 -Description "Test VM for development"
```

This example creates a new VM with a description.

## Related Links

- [Connect-ProxmoxServer](Connect-ProxmoxServer.md)
- [Get-ProxmoxVM](Get-ProxmoxVM.md)
- [Remove-ProxmoxVM](Remove-ProxmoxVM.md)
- [Start-ProxmoxVM](Start-ProxmoxVM.md)
- [Stop-ProxmoxVM](Stop-ProxmoxVM.md)
- [Restart-ProxmoxVM](Restart-ProxmoxVM.md)
- [New-ProxmoxVMBuilder](New-ProxmoxVMBuilder.md)
