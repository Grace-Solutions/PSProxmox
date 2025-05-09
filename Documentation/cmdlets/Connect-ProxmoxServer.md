# Connect-ProxmoxServer

Connects to a Proxmox VE server.

## Syntax

```powershell
Connect-ProxmoxServer
   -Server <String>
   -Username <String>
   -Password <SecureString>
   [-Realm <String>]
   [-Port <Int32>]
   [-UseSSL <Boolean>]
   [-SkipCertificateValidation <Boolean>]
   [-Timeout <Int32>]
   [<CommonParameters>]
```

```powershell
Connect-ProxmoxServer
   -Server <String>
   -Credential <PSCredential>
   [-Realm <String>]
   [-Port <Int32>]
   [-UseSSL <Boolean>]
   [-SkipCertificateValidation <Boolean>]
   [-Timeout <Int32>]
   [<CommonParameters>]
```

```powershell
Connect-ProxmoxServer
   -Server <String>
   -ApiToken <String>
   -ApiTokenSecret <SecureString>
   [-Port <Int32>]
   [-UseSSL <Boolean>]
   [-SkipCertificateValidation <Boolean>]
   [-Timeout <Int32>]
   [<CommonParameters>]
```

## Description

The `Connect-ProxmoxServer` cmdlet establishes a connection to a Proxmox VE server. This connection is required for all other cmdlets in the PSProxmox module.

## Parameters

### -Server

The hostname or IP address of the Proxmox VE server.

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

### -Username

The username to use for authentication.

```yaml
Type: String
Parameter Sets: UsernamePassword
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Password

The password to use for authentication.

```yaml
Type: SecureString
Parameter Sets: UsernamePassword
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Credential

The credential object to use for authentication.

```yaml
Type: PSCredential
Parameter Sets: Credential
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ApiToken

The API token to use for authentication.

```yaml
Type: String
Parameter Sets: ApiToken
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ApiTokenSecret

The API token secret to use for authentication.

```yaml
Type: SecureString
Parameter Sets: ApiToken
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Realm

The authentication realm to use. Default is "pam".

```yaml
Type: String
Parameter Sets: UsernamePassword, Credential
Aliases:

Required: False
Position: Named
Default value: pam
Accept pipeline input: False
Accept wildcard characters: False
```

### -Port

The port to use for the connection. Default is 8006.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 8006
Accept pipeline input: False
Accept wildcard characters: False
```

### -UseSSL

Whether to use SSL for the connection. Default is $true.

```yaml
Type: Boolean
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: True
Accept pipeline input: False
Accept wildcard characters: False
```

### -SkipCertificateValidation

Whether to skip SSL certificate validation. Default is $false.

```yaml
Type: Boolean
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -Timeout

The timeout in seconds for the connection. Default is 30.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 30
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## Inputs

### None

## Outputs

### PSProxmox.Session.ProxmoxConnection

## Notes

- This cmdlet must be called before using any other cmdlets in the PSProxmox module.
- The connection object is returned and automatically stored as the default connection.
- Other cmdlets will automatically use the default connection if no connection is specified.
- You can still store the connection in a variable and pass it explicitly to cmdlets if needed.
- If you use the `-SkipCertificateValidation` parameter, the SSL certificate validation will be skipped, which is not recommended for production environments.

## Examples

### Example 1: Connect to a Proxmox VE server using username and password

```powershell
$securePassword = ConvertTo-SecureString "password" -AsPlainText -Force
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Username "root" -Password $securePassword -Realm "pam"
```

This example connects to a Proxmox VE server using a username and password.

### Example 2: Connect to a Proxmox VE server using a credential object

```powershell
$credential = Get-Credential
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential $credential -Realm "pam"
```

This example connects to a Proxmox VE server using a credential object.

### Example 3: Connect to a Proxmox VE server using an API token

```powershell
$secureSecret = ConvertTo-SecureString "secret" -AsPlainText -Force
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -ApiToken "root@pam!token" -ApiTokenSecret $secureSecret
```

This example connects to a Proxmox VE server using an API token.

### Example 4: Connect to a Proxmox VE server with SSL certificate validation disabled

```powershell
$securePassword = ConvertTo-SecureString "password" -AsPlainText -Force
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Username "root" -Password $securePassword -Realm "pam" -SkipCertificateValidation $true
```

This example connects to a Proxmox VE server with SSL certificate validation disabled.

## Related Links

- [Disconnect-ProxmoxServer](Disconnect-ProxmoxServer.md)
- [Get-ProxmoxVM](Get-ProxmoxVM.md)
- [Get-ProxmoxNode](Get-ProxmoxNode.md)
