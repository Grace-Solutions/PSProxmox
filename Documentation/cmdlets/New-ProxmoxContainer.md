# New-ProxmoxContainer

## SYNOPSIS
Creates a new Proxmox LXC container.

## SYNTAX

```powershell
New-ProxmoxContainer -Node <String> [-Builder <ProxmoxContainerBuilder>] [-CTID <Int32>] [-Name <String>]
 [-OSTemplate <String>] [-Storage <String>] [-Memory <Int32>] [-Swap <Int32>] [-Cores <Int32>]
 [-DiskSize <Int32>] [-Unprivileged] [-Password <SecureString>] [-SSHKey <String>] [-Description <String>]
 [-StartOnBoot] [-Start] [<CommonParameters>]
```

## DESCRIPTION
Creates a new Proxmox LXC container on a Proxmox server.

## EXAMPLES

### Example 1: Create a new container
```powershell
New-ProxmoxContainer -Node "pve1" -Name "web-container" -OSTemplate "local:vztmpl/ubuntu-20.04-standard_20.04-1_amd64.tar.gz" -Storage "local-lvm" -Memory 512 -Swap 512 -Cores 1 -DiskSize 8
```

Creates a new LXC container on node pve1.

### Example 2: Create a new container using a builder
```powershell
$builder = New-ProxmoxContainerBuilder -Name "web-container"
$builder.WithOSTemplate("local:vztmpl/ubuntu-20.04-standard_20.04-1_amd64.tar.gz")
        .WithStorage("local-lvm")
        .WithMemory(512)
        .WithSwap(512)
        .WithCores(1)
        .WithDiskSize(8)
        .WithUnprivileged($true)
        .WithStartOnBoot($true)
        .WithStart($true)
New-ProxmoxContainer -Node "pve1" -Builder $builder
```

Creates a new LXC container on node pve1 using a builder.

## PARAMETERS

### -Node
The node name.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Builder
The container builder.

```yaml
Type: ProxmoxContainerBuilder
Parameter Sets: (All)
Aliases:

Required: False
Position: 1
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -CTID
The container ID.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Name
The container name.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -OSTemplate
The container OS template.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Storage
The container storage.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Memory
The container memory limit in MB.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Swap
The container swap limit in MB.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Cores
The container CPU cores.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -DiskSize
The container disk size in GB.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Unprivileged
Whether the container is unprivileged.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Password
The container password.

```yaml
Type: SecureString
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -SSHKey
The container SSH public key.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Description
The container description.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -StartOnBoot
Whether to start the container on boot.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Start
Whether to start the container after creation.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String
### PSProxmox.Models.ProxmoxContainerBuilder
### System.Int32
### System.Security.SecureString
### System.Management.Automation.SwitchParameter

## OUTPUTS

### PSProxmox.Models.ProxmoxContainer

## NOTES

## RELATED LINKS
