# Get-ProxmoxNode

Gets nodes from a Proxmox VE cluster.

## Syntax

```powershell
Get-ProxmoxNode
   [-Connection <ProxmoxConnection>]
   [-Name <String>]
   [-UseRegex]
   [-RawJson]
   [<CommonParameters>]
```

## Description

The `Get-ProxmoxNode` cmdlet retrieves nodes from a Proxmox VE cluster. You can retrieve all nodes or a specific node by name.

## Parameters

### -Connection

The connection to the Proxmox VE server.

```yaml
Type: ProxmoxConnection
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Name

The name of the node to retrieve. Supports wildcards and regex when used with -UseRegex.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: True
```

### -UseRegex

Use regular expressions for filtering.

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

### PSProxmox.Models.ProxmoxNode

### System.String

## Notes

- This cmdlet requires a connection to a Proxmox VE server. Use `Connect-ProxmoxServer` to establish a connection.
- If no connection is specified, the cmdlet will use the default connection.
- If the `-RawJson` parameter is specified, the raw JSON response will be returned instead of parsed objects.
- If the `-Name` parameter is specified and the node is not found, an error will be thrown.
- The `-Name` parameter supports wildcards (e.g., "pve*") by default.
- Use the `-UseRegex` parameter with `-Name` to filter using regular expressions (e.g., "^pve[0-9]+$").

## Examples

### Example 1: Get all nodes

```powershell
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$nodes = Get-ProxmoxNode
```

This example gets all nodes from the Proxmox VE cluster.

### Example 2: Get a specific node by name

```powershell
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$node = Get-ProxmoxNode -Name "pve1"
```

This example gets the node named "pve1".

### Example 3: Get nodes by name using wildcards

```powershell
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$nodes = Get-ProxmoxNode -Name "pve*"
```

This example gets all nodes with names starting with "pve".

### Example 4: Get nodes by name using regex

```powershell
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$nodes = Get-ProxmoxNode -Name "^pve[0-9]+$" -UseRegex
```

This example gets all nodes with names matching the pattern "pve" followed by one or more digits.

### Example 5: Get the raw JSON response

```powershell
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$json = Get-ProxmoxNode -RawJson
```

This example gets the raw JSON response for all nodes.

## Related Links

- [Connect-ProxmoxServer](Connect-ProxmoxServer.md)
- [Get-ProxmoxVM](Get-ProxmoxVM.md)
