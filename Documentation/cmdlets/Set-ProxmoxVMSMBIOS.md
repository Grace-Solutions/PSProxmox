# Set-ProxmoxVMSMBIOS

Sets the SMBIOS settings for a Proxmox VM.

## Syntax

```powershell
Set-ProxmoxVMSMBIOS
   -Connection <ProxmoxConnection>
   -Node <String>
   -VMID <Int32>
   -SMBIOS <ProxmoxVMSMBIOS>
   [-PassThru]
   [-WhatIf]
   [-Confirm]
   [<CommonParameters>]
```

```powershell
Set-ProxmoxVMSMBIOS
   -Connection <ProxmoxConnection>
   -Node <String>
   -VMID <Int32>
   [-Manufacturer <String>]
   [-Product <String>]
   [-Version <String>]
   [-Serial <String>]
   [-Family <String>]
   [-UUID <String>]
   [-PassThru]
   [-WhatIf]
   [-Confirm]
   [<CommonParameters>]
```

## Description

The `Set-ProxmoxVMSMBIOS` cmdlet sets the SMBIOS (System Management BIOS) settings for a virtual machine in Proxmox VE. SMBIOS settings allow you to customize hardware identification information that's presented to the guest operating system.

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

The node where the VM is located.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -VMID

The ID of the VM.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: True
Position: 2
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SMBIOS

The SMBIOS settings object.

```yaml
Type: ProxmoxVMSMBIOS
Parameter Sets: SMBIOSObject
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Manufacturer

The manufacturer information.

```yaml
Type: String
Parameter Sets: Individual
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Product

The product information.

```yaml
Type: String
Parameter Sets: Individual
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Version

The version information.

```yaml
Type: String
Parameter Sets: Individual
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Serial

The serial number.

```yaml
Type: String
Parameter Sets: Individual
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Family

The system family.

```yaml
Type: String
Parameter Sets: Individual
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -UUID

The system UUID.

```yaml
Type: String
Parameter Sets: Individual
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PassThru

Return the updated SMBIOS settings.

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

### -WhatIf

Shows what would happen if the cmdlet runs. The cmdlet is not run.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: wi

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Confirm

Prompts you for confirmation before running the cmdlet.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: cf

Required: False
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

### PSProxmox.Models.ProxmoxVMSMBIOS

Returns a ProxmoxVMSMBIOS object if -PassThru is specified.

## Notes

- SMBIOS settings can be useful for software licensing that's tied to hardware identifiers
- These settings can also help with VM migration between different hypervisors
- Some applications may require specific hardware information to function properly

## Examples

### Example 1: Set individual SMBIOS settings for a VM

```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
Set-ProxmoxVMSMBIOS -Connection $connection -Node "pve1" -VMID 100 -Manufacturer "Dell Inc." -Product "PowerEdge R740" -Serial "ABC123"
```

This example sets the manufacturer, product, and serial number SMBIOS settings for VM 100 on node pve1.

### Example 2: Set SMBIOS settings using an object

```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$smbios = New-Object PSProxmox.Models.ProxmoxVMSMBIOS
$smbios.Manufacturer = "Dell Inc."
$smbios.Product = "PowerEdge R740"
$smbios.Serial = "ABC123"
Set-ProxmoxVMSMBIOS -Connection $connection -Node "pve1" -VMID 100 -SMBIOS $smbios
```

This example creates a ProxmoxVMSMBIOS object and uses it to set the SMBIOS settings for VM 100.

### Example 3: Get current SMBIOS settings, modify them, and update

```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$smbios = Get-ProxmoxVMSMBIOS -Connection $connection -Node "pve1" -VMID 100
$smbios.Manufacturer = "Dell Inc."
$smbios.Product = "PowerEdge R740"
Set-ProxmoxVMSMBIOS -Connection $connection -Node "pve1" -VMID 100 -SMBIOS $smbios -PassThru
```

This example retrieves the current SMBIOS settings for VM 100, modifies them, and then updates the VM with the new settings.

## Related Links

- [Get-ProxmoxVMSMBIOS](Get-ProxmoxVMSMBIOS.md)
- [New-ProxmoxVMBuilder](New-ProxmoxVMBuilder.md)
- [New-ProxmoxVM](New-ProxmoxVM.md)
