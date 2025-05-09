# Set-ProxmoxVMCloudInit

Sets Cloud-Init configuration for a VM.

## Syntax

```powershell
Set-ProxmoxVMCloudInit
   -Node <String>
   -VMID <Int32>
   [-Username <String>]
   [-Password <SecureString>]
   [-SSHKey <String>]
   [-IPConfig <String>]
   [-DNS <String>]
   [-Connection <ProxmoxConnection>]
   [<CommonParameters>]
```

## Description

The `Set-ProxmoxVMCloudInit` cmdlet sets Cloud-Init configuration for a VM. Cloud-Init is used to customize cloud images at boot time.

## Parameters

### -Node

The node on which the VM is located.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
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
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Username

The username for Cloud-Init.

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

### -Password

The password for Cloud-Init.

```yaml
Type: SecureString
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SSHKey

The SSH public key for Cloud-Init.

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

### -IPConfig

The IP configuration for Cloud-Init (e.g., "dhcp" or "ip=192.168.1.100/24,gw=192.168.1.1").

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

### -DNS

The DNS servers for Cloud-Init (comma-separated).

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

### -Connection

The connection to the Proxmox VE server.

```yaml
Type: ProxmoxConnection
Parameter Sets: (All)
Aliases:

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

### PSProxmox.Models.ProxmoxVM

## Notes

- This cmdlet requires a connection to a Proxmox VE server. Use `Connect-ProxmoxServer` to establish a connection.
- The VM must have a Cloud-Init drive attached.
- The Cloud-Init configuration is applied when the VM is started.

## Examples

### Example 1: Set Cloud-Init configuration for a VM

```powershell
$password = ConvertTo-SecureString "password" -AsPlainText -Force
Set-ProxmoxVMCloudInit -Node "pve1" -VMID 100 -Username "admin" -Password $password -SSHKey "ssh-rsa AAAA..." -IPConfig "dhcp" -DNS "8.8.8.8,8.8.4.4"
```

This example sets Cloud-Init configuration for VM 100 on node "pve1" with the specified username, password, SSH key, IP configuration, and DNS servers.

### Example 2: Set Cloud-Init configuration with static IP

```powershell
$password = ConvertTo-SecureString "password" -AsPlainText -Force
Set-ProxmoxVMCloudInit -Node "pve1" -VMID 100 -Username "admin" -Password $password -IPConfig "ip=192.168.1.100/24,gw=192.168.1.1"
```

This example sets Cloud-Init configuration for VM 100 on node "pve1" with a static IP address.

### Example 3: Set Cloud-Init configuration with SSH key only

```powershell
Set-ProxmoxVMCloudInit -Node "pve1" -VMID 100 -Username "admin" -SSHKey "ssh-rsa AAAA..."
```

This example sets Cloud-Init configuration for VM 100 on node "pve1" with the specified username and SSH key.

## Related Links

- [New-ProxmoxCloudImageTemplate](New-ProxmoxCloudImageTemplate.md)
- [New-ProxmoxVMFromTemplate](New-ProxmoxVMFromTemplate.md)
