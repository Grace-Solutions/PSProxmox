# Get-ProxmoxIPPool

Gets IP address pools.

## Syntax

```powershell
Get-ProxmoxIPPool
   [-Name <String>]
   [-UseRegex]
   [<CommonParameters>]
```

## Description

The `Get-ProxmoxIPPool` cmdlet retrieves IP address pools used with Proxmox VE virtual machines. You can retrieve all pools or a specific pool by name.

## Parameters

### -Name

The name of the IP pool to retrieve. Supports wildcards and regex when used with -UseRegex.

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

### PSProxmox.Models.ProxmoxIPPool

## Notes

- This cmdlet does not require a connection to a Proxmox VE server as IP pools are stored locally.
- If the `-Name` parameter is specified and the pool is not found, an error will be thrown.
- The `-Name` parameter supports wildcards (e.g., "Production*") by default.
- Use the `-UseRegex` parameter with `-Name` to filter using regular expressions (e.g., "^Prod[a-z]+$").

## Examples

### Example 1: Get all IP pools

```powershell
$pools = Get-ProxmoxIPPool
```

This example gets all IP pools.

### Example 2: Get a specific IP pool by name

```powershell
$pool = Get-ProxmoxIPPool -Name "Production"
```

This example gets the IP pool named "Production".

### Example 3: Get IP pools by name using wildcards

```powershell
$pools = Get-ProxmoxIPPool -Name "Prod*"
```

This example gets all IP pools with names starting with "Prod".

### Example 4: Get IP pools by name using regex

```powershell
$pools = Get-ProxmoxIPPool -Name "^Prod[a-z]+$" -UseRegex
```

This example gets all IP pools with names matching the pattern "Prod" followed by one or more lowercase letters.

## Related Links

- [New-ProxmoxIPPool](New-ProxmoxIPPool.md)
- [Clear-ProxmoxIPPool](Clear-ProxmoxIPPool.md)
