# Disconnect-ProxmoxServer

Disconnects from a Proxmox VE server.

## Syntax

```powershell
Disconnect-ProxmoxServer
   -Connection <ProxmoxConnection>
   [<CommonParameters>]
```

## Description

The `Disconnect-ProxmoxServer` cmdlet terminates a connection to a Proxmox VE server. If the connection being disconnected is the current default connection, it will also be cleared from the default connection.

## Parameters

### -Connection

The connection to disconnect from.

```yaml
Type: ProxmoxConnection
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## Inputs

### PSProxmox.Session.ProxmoxConnection

## Outputs

### None

## Notes

- This cmdlet terminates the connection to the Proxmox VE server.
- If the connection being disconnected is the current default connection, it will also be cleared from the default connection.
- After disconnecting, you will need to call `Connect-ProxmoxServer` again to establish a new connection.

## Examples

### Example 1: Disconnect from a Proxmox VE server

```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
Disconnect-ProxmoxServer -Connection $connection
```

This example disconnects from a Proxmox VE server.

### Example 2: Connect, use the default connection, and disconnect

```powershell
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
Get-ProxmoxVM # Uses the default connection automatically
$connection = Test-ProxmoxConnection -Detailed # Get the current connection
Disconnect-ProxmoxServer -Connection $connection
```

This example connects to a Proxmox VE server, uses the default connection with Get-ProxmoxVM, and then disconnects.

## Related Links

- [Connect-ProxmoxServer](Connect-ProxmoxServer.md)
- [Test-ProxmoxConnection](Test-ProxmoxConnection.md)
