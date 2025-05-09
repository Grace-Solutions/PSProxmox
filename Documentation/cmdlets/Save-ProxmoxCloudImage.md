# Save-ProxmoxCloudImage

Downloads a cloud image.

## Syntax

```powershell
Save-ProxmoxCloudImage
   -Distribution <String>
   -Release <String>
   [-Variant <String>]
   [-OutputPath <String>]
   [-Force]
   [<CommonParameters>]
```

## Description

The `Save-ProxmoxCloudImage` cmdlet downloads a cloud image from a repository. You can specify the distribution, release, and variant to download.

## Parameters

### -Distribution

The distribution to download (e.g., "ubuntu", "debian").

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

### -Release

The release version to download (e.g., "22.04", "11").

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

### -Variant

The image variant to download (e.g., "server", "minimal").

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

### -OutputPath

The output path where the image will be saved. If not specified, the image will be saved to the default download directory.

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

### -Force

Force download even if the image already exists.

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

### System.String

The path to the downloaded image.

## Notes

- This cmdlet downloads cloud images to a local cache directory by default. Use the `-OutputPath` parameter to specify a different location.
- The cmdlet supports Ubuntu and Debian cloud images.
- The cmdlet shows a progress bar during the download.

## Examples

### Example 1: Download an Ubuntu 22.04 server cloud image

```powershell
Save-ProxmoxCloudImage -Distribution "ubuntu" -Release "22.04" -Variant "server"
```

This example downloads an Ubuntu 22.04 server cloud image to the default download directory.

### Example 2: Download a Debian 11 generic cloud image to a specific location

```powershell
Save-ProxmoxCloudImage -Distribution "debian" -Release "11" -Variant "generic" -OutputPath "C:\Images\debian-11.qcow2"
```

This example downloads a Debian 11 generic cloud image to the specified location.

### Example 3: Force download of an Ubuntu 22.04 server cloud image

```powershell
Save-ProxmoxCloudImage -Distribution "ubuntu" -Release "22.04" -Variant "server" -Force
```

This example forces the download of an Ubuntu 22.04 server cloud image, even if it already exists in the cache.

## Related Links

- [Get-ProxmoxCloudImage](Get-ProxmoxCloudImage.md)
- [Invoke-ProxmoxCloudImageCustomization](Invoke-ProxmoxCloudImageCustomization.md)
- [New-ProxmoxCloudImageTemplate](New-ProxmoxCloudImageTemplate.md)
