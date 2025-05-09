# Get-ProxmoxVMTemplate

Gets virtual machine templates.

## Syntax

```powershell
Get-ProxmoxVMTemplate
   [-Name <String>]
   [-UseRegex]
   [<CommonParameters>]
```

## Description

The `Get-ProxmoxVMTemplate` cmdlet retrieves virtual machine templates. You can retrieve all templates or a specific template by name.

## Parameters

### -Name

The name of the template to retrieve. Supports wildcards and regex when used with -UseRegex.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
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

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## Inputs

### None

## Outputs

### PSProxmox.Models.ProxmoxVMTemplate

## Notes

- This cmdlet does not require a connection to a Proxmox VE server as templates are stored locally.
- If the `-Name` parameter is specified and the template is not found, an error will be thrown.
- The `-Name` parameter supports wildcards (e.g., "Ubuntu*") by default.
- Use the `-UseRegex` parameter with `-Name` to filter using regular expressions (e.g., "^Ubuntu-[0-9]+\\.[0-9]+$").

## Examples

### Example 1: Get all templates

```powershell
$templates = Get-ProxmoxVMTemplate
```

This example gets all templates.

### Example 2: Get a specific template by name

```powershell
$template = Get-ProxmoxVMTemplate -Name "Ubuntu-Template"
```

This example gets the template named "Ubuntu-Template".

### Example 3: Get templates by name using wildcards

```powershell
$templates = Get-ProxmoxVMTemplate -Name "Ubuntu*"
```

This example gets all templates with names starting with "Ubuntu".

### Example 4: Get templates by name using regex

```powershell
$templates = Get-ProxmoxVMTemplate -Name "^Ubuntu-[0-9]+\\.[0-9]+$" -UseRegex
```

This example gets all templates with names matching the pattern "Ubuntu-" followed by a version number (e.g., "Ubuntu-20.04").

## Related Links

- [New-ProxmoxVMTemplate](New-ProxmoxVMTemplate.md)
- [Remove-ProxmoxVMTemplate](Remove-ProxmoxVMTemplate.md)
- [New-ProxmoxVMFromTemplate](New-ProxmoxVMFromTemplate.md)
