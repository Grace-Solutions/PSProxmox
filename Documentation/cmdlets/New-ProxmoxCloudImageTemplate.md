# New-ProxmoxCloudImageTemplate

Creates a template from a cloud image.

## Syntax

```powershell
New-ProxmoxCloudImageTemplate
   -Node <String>
   -Name <String>
   -Distribution <String>
   -Release <String>
   [-Variant <String>]
   -Storage <String>
   [-Memory <Int32>]
   [-Cores <Int32>]
   [-DiskSize <Int32>]
   [-NetworkType <String>]
   [-Bridge <String>]
   [-ScsiController <String>]
   [-Connection <ProxmoxConnection>]
   [<CommonParameters>]
```

```powershell
New-ProxmoxCloudImageTemplate
   -Node <String>
   -Name <String>
   -ImagePath <String>
   -Storage <String>
   [-Memory <Int32>]
   [-Cores <Int32>]
   [-DiskSize <Int32>]
   [-NetworkType <String>]
   [-Bridge <String>]
   [-ScsiController <String>]
   [-Connection <ProxmoxConnection>]
   [<CommonParameters>]
```

## Description

The `New-ProxmoxCloudImageTemplate` cmdlet creates a template from a cloud image. You can either specify a distribution, release, and variant to download a cloud image automatically, or provide a path to a local cloud image file.

## Parameters

### -Node

The node on which to create the template.

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

### -Name

The name of the template.

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

### -Distribution

The distribution of the cloud image (e.g., "ubuntu", "debian").

```yaml
Type: String
Parameter Sets: ByDistribution
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Release

The release version of the cloud image (e.g., "22.04", "11").

```yaml
Type: String
Parameter Sets: ByDistribution
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Variant

The variant of the cloud image (e.g., "server", "minimal").

```yaml
Type: String
Parameter Sets: ByDistribution
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ImagePath

The path to a local cloud image file.

```yaml
Type: String
Parameter Sets: ByImagePath
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Storage

The storage on which to create the template.

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

### -Memory

The amount of memory in MB for the template.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 1024
Accept pipeline input: False
Accept wildcard characters: False
```

### -Cores

The number of CPU cores for the template.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 1
Accept pipeline input: False
Accept wildcard characters: False
```

### -DiskSize

The disk size in GB for the template.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: 10
Accept pipeline input: False
Accept wildcard characters: False
```

### -NetworkType

The network interface type for the template.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: virtio
Accept pipeline input: False
Accept wildcard characters: False
```

### -Bridge

The network bridge for the template.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: vmbr0
Accept pipeline input: False
Accept wildcard characters: False
```

### -ScsiController

The SCSI controller type for the template.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: virtio-scsi-pci
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
- The cmdlet creates a VM from a cloud image and converts it to a template.
- The template includes a Cloud-Init drive for easy customization.
- The cmdlet shows progress bars during the download and upload processes.

## Examples

### Example 1: Create a template from an Ubuntu 22.04 cloud image

```powershell
New-ProxmoxCloudImageTemplate -Node "pve1" -Name "ubuntu-22.04" -Distribution "ubuntu" -Release "22.04" -Storage "local-lvm" -Memory 2048 -Cores 2 -DiskSize 20
```

This example creates a template named "ubuntu-22.04" from an Ubuntu 22.04 cloud image on the node "pve1".

### Example 2: Create a template from a local cloud image file

```powershell
New-ProxmoxCloudImageTemplate -Node "pve1" -Name "ubuntu-22.04" -ImagePath "C:\Images\ubuntu-22.04-server-cloudimg-amd64.img" -Storage "local-lvm" -Memory 2048 -Cores 2 -DiskSize 20
```

This example creates a template named "ubuntu-22.04" from a local cloud image file on the node "pve1".

### Example 3: Create a template with custom network settings

```powershell
New-ProxmoxCloudImageTemplate -Node "pve1" -Name "ubuntu-22.04" -Distribution "ubuntu" -Release "22.04" -Storage "local-lvm" -Memory 2048 -Cores 2 -DiskSize 20 -NetworkType "virtio" -Bridge "vmbr1"
```

This example creates a template named "ubuntu-22.04" from an Ubuntu 22.04 cloud image on the node "pve1" with custom network settings.

## Related Links

- [Get-ProxmoxCloudImage](Get-ProxmoxCloudImage.md)
- [Save-ProxmoxCloudImage](Save-ProxmoxCloudImage.md)
- [Set-ProxmoxVMCloudInit](Set-ProxmoxVMCloudInit.md)
- [New-ProxmoxVMFromTemplate](New-ProxmoxVMFromTemplate.md)
