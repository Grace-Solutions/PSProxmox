# Get-ProxmoxVMSMBIOS

Gets the SMBIOS settings for a Proxmox VM.

## Syntax

```powershell
Get-ProxmoxVMSMBIOS
   -Connection <ProxmoxConnection>
   -Node <String>
   -VMID <Int32>
   [-RawJson]
   [<CommonParameters>]
```

## Description

The `Get-ProxmoxVMSMBIOS` cmdlet retrieves the SMBIOS (System Management BIOS) settings for a virtual machine in Proxmox VE. SMBIOS settings allow you to customize hardware identification information that's presented to the guest operating system.

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

### -RawJson

Return the raw JSON response from the Proxmox API instead of a PowerShell object.

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

### PSProxmox.Models.ProxmoxVMSMBIOS

Returns a ProxmoxVMSMBIOS object containing the SMBIOS settings for the VM.

## Notes

- SMBIOS settings can be useful for software licensing that's tied to hardware identifiers
- These settings can also help with VM migration between different hypervisors
- Some applications may require specific hardware information to function properly

## Examples

### Example 1: Get SMBIOS settings for a VM

```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
Get-ProxmoxVMSMBIOS -Connection $connection -Node "pve1" -VMID 100
```

This example retrieves the SMBIOS settings for VM 100 on node pve1.

### Example 2: Get SMBIOS settings and display specific properties

```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$smbios = Get-ProxmoxVMSMBIOS -Connection $connection -Node "pve1" -VMID 100
$smbios | Select-Object Manufacturer, Product, Serial
```

This example retrieves the SMBIOS settings for VM 100 and displays only the manufacturer, product, and serial number.

### Example 3: Get SMBIOS settings as raw JSON

```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
Get-ProxmoxVMSMBIOS -Connection $connection -Node "pve1" -VMID 100 -RawJson
```

This example retrieves the SMBIOS settings for VM 100 as raw JSON.

## Related Links

- [Set-ProxmoxVMSMBIOS](Set-ProxmoxVMSMBIOS.md)
- [New-ProxmoxVMBuilder](New-ProxmoxVMBuilder.md)
- [New-ProxmoxVM](New-ProxmoxVM.md)
