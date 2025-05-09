# Test-ProxmoxConnection

Tests a connection to a Proxmox VE server.

## Syntax

```powershell
Test-ProxmoxConnection
   [-Connection <ProxmoxConnection>]
   [-Detailed]
   [<CommonParameters>]
```

## Description

The `Test-ProxmoxConnection` cmdlet tests if a connection to a Proxmox VE server is valid and active. If no connection is specified, the cmdlet will use the current default connection.

## Parameters

### -Connection

The connection to test. If not specified, the current default connection will be used.

```yaml
Type: ProxmoxConnection
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Detailed

Return detailed connection information instead of a boolean.

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

### PSProxmox.Session.ProxmoxConnection

## Outputs

### System.Boolean

Returns `$true` if the connection is valid and active, `$false` otherwise.

### System.Management.Automation.PSObject

When the `-Detailed` parameter is specified, returns a custom object with detailed connection information.

## Notes

- If no connection is specified and no default connection exists, the cmdlet will return an error.
- The cmdlet tests the connection by making a simple API call to the Proxmox VE server.

## Examples

### Example 1: Test the current connection

```powershell
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
Test-ProxmoxConnection
```

This example tests the current default connection.

### Example 2: Test a specific connection

```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
Test-ProxmoxConnection -Connection $connection
```

This example tests a specific connection.

### Example 3: Get detailed connection information

```powershell
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
Test-ProxmoxConnection -Detailed
```

This example returns detailed information about the current default connection.

## Related Links

- [Connect-ProxmoxServer](Connect-ProxmoxServer.md)
- [Disconnect-ProxmoxServer](Disconnect-ProxmoxServer.md)
