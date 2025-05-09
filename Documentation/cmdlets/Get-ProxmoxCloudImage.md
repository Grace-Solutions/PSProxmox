# Get-ProxmoxCloudImage

Gets available cloud images from repositories.

## Syntax

```powershell
Get-ProxmoxCloudImage
   [-Distribution <String>]
   [-Release <String>]
   [-Variant <String>]
   [-Force]
   [<CommonParameters>]
```

## Description

The `Get-ProxmoxCloudImage` cmdlet gets available cloud images from repositories. You can filter the results by distribution, release, and variant.

## Parameters

### -Distribution

The distribution to filter by (e.g., "ubuntu", "debian").

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Release

The release version to filter by (e.g., "22.04", "11").

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Variant

The image variant to filter by (e.g., "server", "minimal").

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 2
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Force

Force refresh of the cloud image cache.

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

### PSProxmox.CloudImages.CloudImage

## Notes

- This cmdlet caches the cloud image metadata to improve performance. Use the `-Force` parameter to refresh the cache.
- The cmdlet supports Ubuntu and Debian cloud images.

## Examples

### Example 1: Get all available cloud images

```powershell
Get-ProxmoxCloudImage
```

This example gets all available cloud images from all supported repositories.

### Example 2: Get Ubuntu cloud images

```powershell
Get-ProxmoxCloudImage -Distribution "ubuntu"
```

This example gets all available Ubuntu cloud images.

### Example 3: Get Ubuntu 22.04 cloud images

```powershell
Get-ProxmoxCloudImage -Distribution "ubuntu" -Release "22.04"
```

This example gets all available Ubuntu 22.04 cloud images.

### Example 4: Get Ubuntu 22.04 server cloud images

```powershell
Get-ProxmoxCloudImage -Distribution "ubuntu" -Release "22.04" -Variant "server"
```

This example gets all available Ubuntu 22.04 server cloud images.

### Example 5: Force refresh of the cloud image cache

```powershell
Get-ProxmoxCloudImage -Force
```

This example forces a refresh of the cloud image cache and gets all available cloud images.

## Related Links

- [Save-ProxmoxCloudImage](Save-ProxmoxCloudImage.md)
- [New-ProxmoxCloudImageTemplate](New-ProxmoxCloudImageTemplate.md)
