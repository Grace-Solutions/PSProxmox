# Invoke-ProxmoxCloudImageCustomization

Customizes a cloud image.

## Syntax

```powershell
Invoke-ProxmoxCloudImageCustomization
   -ImagePath <String>
   [-Resize <Int32>]
   [-ConvertTo <String>]
   [-Packages <String[]>]
   [-Commands <String[]>]
   [-Scripts <String[]>]
   [-OutputPath <String>]
   [<CommonParameters>]
```

## Description

The `Invoke-ProxmoxCloudImageCustomization` cmdlet customizes a cloud image by resizing it, converting it to a different format, adding packages, or running commands or scripts.

## Parameters

### -ImagePath

The path to the cloud image file.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Resize

The new size of the image in GB.

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

### -ConvertTo

The format to convert the image to (e.g., "qcow2", "raw").

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

### -Packages

The packages to install in the image.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Commands

The commands to run in the image.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Scripts

The scripts to run in the image.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -OutputPath

The output path for the customized image. If not specified, the original image will be modified.

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

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## Inputs

### System.String

The path to the cloud image file.

## Outputs

### System.String

The path to the customized image.

## Notes

- This cmdlet requires the QEMU tools to be installed on the system.
- The `-Packages`, `-Commands`, and `-Scripts` parameters are not fully implemented yet.
- The cmdlet shows a progress bar during the customization process.

## Examples

### Example 1: Resize a cloud image to 20GB

```powershell
Invoke-ProxmoxCloudImageCustomization -ImagePath "C:\Images\ubuntu-22.04-server-cloudimg-amd64.img" -Resize 20
```

This example resizes the specified cloud image to 20GB.

### Example 2: Convert a cloud image to qcow2 format

```powershell
Invoke-ProxmoxCloudImageCustomization -ImagePath "C:\Images\ubuntu-22.04-server-cloudimg-amd64.img" -ConvertTo "qcow2"
```

This example converts the specified cloud image to qcow2 format.

### Example 3: Resize and convert a cloud image

```powershell
Invoke-ProxmoxCloudImageCustomization -ImagePath "C:\Images\ubuntu-22.04-server-cloudimg-amd64.img" -Resize 20 -ConvertTo "qcow2" -OutputPath "C:\Images\ubuntu-22.04-server-cloudimg-amd64-custom.qcow2"
```

This example resizes the specified cloud image to 20GB, converts it to qcow2 format, and saves it to the specified output path.

### Example 4: Customize a cloud image with packages and commands

```powershell
Invoke-ProxmoxCloudImageCustomization -ImagePath "C:\Images\ubuntu-22.04-server-cloudimg-amd64.img" -Packages "nginx", "postgresql" -Commands "systemctl enable nginx", "systemctl enable postgresql"
```

This example installs the specified packages and runs the specified commands in the cloud image.

## Related Links

- [Save-ProxmoxCloudImage](Save-ProxmoxCloudImage.md)
- [New-ProxmoxCloudImageTemplate](New-ProxmoxCloudImageTemplate.md)
