# Get-ProxmoxVM

Gets virtual machines from Proxmox VE.

## Syntax

```powershell
Get-ProxmoxVM
   -Connection <ProxmoxConnection>
   [-Node <String>]
   [-VMID <Int32>]
   [-RawJson]
   [<CommonParameters>]
```

## Description

The `Get-ProxmoxVM` cmdlet retrieves virtual machines from Proxmox VE. You can retrieve all VMs, VMs on a specific node, or a specific VM by ID.

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

The node to get VMs from. If not specified, VMs from all nodes will be returned.

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

### -VMID

The ID of the VM to get. If not specified, all VMs will be returned.

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

### -RawJson

Whether to return the raw JSON response instead of parsed objects.

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

### System.String

## Notes

- This cmdlet requires a connection to a Proxmox VE server. Use `Connect-ProxmoxServer` to establish a connection.
- If the `-RawJson` parameter is specified, the raw JSON response will be returned instead of parsed objects.
- If the `-VMID` parameter is specified and the VM is not found, an error will be thrown.

## Examples

### Example 1: Get all VMs

```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$vms = Get-ProxmoxVM -Connection $connection
```

This example gets all VMs from all nodes.

### Example 2: Get VMs on a specific node

```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$vms = Get-ProxmoxVM -Connection $connection -Node "pve1"
```

This example gets all VMs on the node "pve1".

### Example 3: Get a specific VM by ID

```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$vm = Get-ProxmoxVM -Connection $connection -VMID 100
```

This example gets the VM with ID 100.

### Example 4: Get the raw JSON response

```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$json = Get-ProxmoxVM -Connection $connection -RawJson
```

This example gets the raw JSON response for all VMs.

## Related Links

- [Connect-ProxmoxServer](Connect-ProxmoxServer.md)
- [New-ProxmoxVM](New-ProxmoxVM.md)
- [Remove-ProxmoxVM](Remove-ProxmoxVM.md)
- [Start-ProxmoxVM](Start-ProxmoxVM.md)
- [Stop-ProxmoxVM](Stop-ProxmoxVM.md)
- [Restart-ProxmoxVM](Restart-ProxmoxVM.md)
