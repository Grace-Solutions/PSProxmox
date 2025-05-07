# New-ProxmoxVMFromTemplate

Creates a new virtual machine from a template in Proxmox VE.

## Syntax

```powershell
New-ProxmoxVMFromTemplate
   -Connection <ProxmoxConnection>
   -Node <String>
   -TemplateName <String>
   -Name <String>
   [-VMID <Int32>]
   [-Memory <Int32>]
   [-Cores <Int32>]
   [-DiskSize <Int32>]
   [-Storage <String>]
   [-NetworkModel <String>]
   [-NetworkBridge <String>]
   [-Description <String>]
   [-IPPool <String>]
   [-AutomaticSMBIOS]
   [-SMBIOSProfile <String>]
   [-Start]
   [<CommonParameters>]
```

```powershell
New-ProxmoxVMFromTemplate
   -Connection <ProxmoxConnection>
   -Node <String>
   -TemplateName <String>
   -Prefix <String>
   -Count <Int32>
   [-StartIndex <Int32>]
   [-CounterDigits <Int32>]
   [-Memory <Int32>]
   [-Cores <Int32>]
   [-DiskSize <Int32>]
   [-Storage <String>]
   [-NetworkModel <String>]
   [-NetworkBridge <String>]
   [-Description <String>]
   [-IPPool <String>]
   [-AutomaticSMBIOS]
   [-SMBIOSProfile <String>]
   [-Start]
   [<CommonParameters>]
```

## Description

The `New-ProxmoxVMFromTemplate` cmdlet creates a new virtual machine from a template in Proxmox VE. You can create a single VM or multiple VMs with a prefix and counter.

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

### -TemplateName

The name of the template to use.

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
Parameter Sets: SingleVM
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Prefix

The prefix for the VM names when creating multiple VMs.

```yaml
Type: String
Parameter Sets: MultipleVMs
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Count

The number of VMs to create.

```yaml
Type: Int32
Parameter Sets: MultipleVMs
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -StartIndex

The starting index for the VM names when creating multiple VMs.

```yaml
Type: Int32
Parameter Sets: MultipleVMs
Aliases:

Required: False
Position: Named
Default value: 1
Accept pipeline input: False
Accept wildcard characters: False
```

### -CounterDigits

The number of digits to use for the counter in VM names (e.g., 3 would result in "Prefix-001").

```yaml
Type: Int32
Parameter Sets: MultipleVMs
Aliases:

Required: False
Position: Named
Default value: 1
Accept pipeline input: False
Accept wildcard characters: False
```

### -VMID

The VM ID. If not specified, the next available ID will be used.

```yaml
Type: Int32
Parameter Sets: SingleVM
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Memory

The amount of memory in MB. If specified, overrides the template value.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Cores

The number of CPU cores. If specified, overrides the template value.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -DiskSize

The disk size in GB. If specified, overrides the template value.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Storage

The storage location for the disk. If specified, overrides the template value.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -NetworkModel

The network interface model. If specified, overrides the template value.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -NetworkBridge

The network bridge. If specified, overrides the template value.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Description

The description of the VM. If specified, overrides the template value.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -AutomaticSMBIOS

Whether to automatically generate SMBIOS values.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -SMBIOSProfile

The manufacturer profile to use for SMBIOS values. Valid values are: Proxmox, Dell, HP, Lenovo, Microsoft, VMware, HyperV, VirtualBox, Random.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: Random
Accept pipeline input: False
Accept wildcard characters: False
```

### -IPPool

The IP pool to use for assigning an IP address.

```yaml
Type: String
Parameter Sets: (All)
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
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
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
- If the `-IPPool` parameter is specified, an IP address will be assigned from the specified pool.
- When creating multiple VMs, the names will be in the format `{Prefix}{FormattedCounter}` where `FormattedCounter` is the counter (StartIndex + i) formatted with the specified number of digits.
- If the `-CounterDigits` parameter is specified, the counter will be padded with leading zeros to the specified number of digits (e.g., "Prefix-001").
- If the `-AutomaticSMBIOS` parameter is specified, SMBIOS values will be automatically generated using the specified profile.
- The `-SMBIOSProfile` parameter specifies the manufacturer profile to use for SMBIOS values. Valid values are: Proxmox, Dell, HP, Lenovo, Microsoft, VMware, HyperV, VirtualBox, Random.

## Examples

### Example 1: Create a VM from a template

```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$vm = New-ProxmoxVMFromTemplate -Connection $connection -Node "pve1" -TemplateName "Ubuntu-Template" -Name "web01" -Start
```

This example creates a new VM from the "Ubuntu-Template" template and starts it.

### Example 2: Create multiple VMs from a template

```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$vms = New-ProxmoxVMFromTemplate -Connection $connection -Node "pve1" -TemplateName "Ubuntu-Template" -Prefix "web" -Count 3 -Start
```

This example creates three new VMs from the "Ubuntu-Template" template with names "web1", "web2", and "web3", and starts them.

### Example 3: Create a VM from a template with custom settings

```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$vm = New-ProxmoxVMFromTemplate -Connection $connection -Node "pve1" -TemplateName "Ubuntu-Template" -Name "web01" -Memory 4096 -Cores 2 -DiskSize 50 -Start
```

This example creates a new VM from the "Ubuntu-Template" template with 4 GB of memory, 2 CPU cores, and a 50 GB disk, and starts it.

### Example 4: Create a VM from a template with an IP from a pool

```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$vm = New-ProxmoxVMFromTemplate -Connection $connection -Node "pve1" -TemplateName "Ubuntu-Template" -Name "web01" -IPPool "Production" -Start
```

This example creates a new VM from the "Ubuntu-Template" template, assigns an IP address from the "Production" pool, and starts it.

### Example 5: Create multiple VMs with padded counters

```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$vms = New-ProxmoxVMFromTemplate -Connection $connection -Node "pve1" -TemplateName "Ubuntu-Template" -Prefix "web-" -Count 5 -CounterDigits 3 -Start
```

This example creates five new VMs from the "Ubuntu-Template" template with names "web-001", "web-002", "web-003", "web-004", and "web-005", and starts them.

### Example 6: Create VMs with automatic SMBIOS settings

```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$vms = New-ProxmoxVMFromTemplate -Connection $connection -Node "pve1" -TemplateName "Windows-Template" -Prefix "win-" -Count 3 -AutomaticSMBIOS -SMBIOSProfile "Dell" -Start
```

This example creates three new VMs from the "Windows-Template" template with Dell SMBIOS settings, which can be useful for software licensing that's tied to hardware identifiers.

## Related Links

- [Connect-ProxmoxServer](Connect-ProxmoxServer.md)
- [Get-ProxmoxVM](Get-ProxmoxVM.md)
- [New-ProxmoxVM](New-ProxmoxVM.md)
- [Get-ProxmoxVMTemplate](Get-ProxmoxVMTemplate.md)
- [New-ProxmoxVMTemplate](New-ProxmoxVMTemplate.md)
- [New-ProxmoxIPPool](New-ProxmoxIPPool.md)
