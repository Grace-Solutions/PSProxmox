# Get-ProxmoxTurnKeyTemplate

## SYNOPSIS
Gets TurnKey Linux templates for Proxmox LXC containers.

## SYNTAX

```powershell
Get-ProxmoxTurnKeyTemplate [[-Node] <String>] [[-Name] <String>] [-IncludeDownloaded] [<CommonParameters>]
```

## DESCRIPTION
Gets TurnKey Linux templates for Proxmox LXC containers from a Proxmox server.

If no parameters are specified, all templates are returned.
If a node is specified, only templates available on that node are returned.
If a name is specified, only templates with that name are returned (supports wildcards and regex).

## EXAMPLES

### Example 1: Get all TurnKey templates
```powershell
Get-ProxmoxTurnKeyTemplate
```

Gets all TurnKey Linux templates from the Proxmox server.

### Example 2: Get all TurnKey templates on a specific node
```powershell
Get-ProxmoxTurnKeyTemplate -Node "pve1"
```

Gets all TurnKey Linux templates available on node pve1.

### Example 3: Get TurnKey templates by name using wildcards
```powershell
Get-ProxmoxTurnKeyTemplate -Name "wordpress*"
```

Gets all TurnKey Linux templates with names starting with "wordpress" (wildcard).

### Example 4: Get TurnKey templates by name using regex
```powershell
Get-ProxmoxTurnKeyTemplate -Name "^wordpress\d+$"
```

Gets all TurnKey Linux templates with names matching the regex pattern "^wordpress\d+$".

### Example 5: Include downloaded templates
```powershell
Get-ProxmoxTurnKeyTemplate -IncludeDownloaded
```

Gets all TurnKey Linux templates, including those that have already been downloaded.

## PARAMETERS

### -Node
The node name.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Name
The template name (supports wildcards and regex).

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: True
```

### -IncludeDownloaded
Whether to include downloaded templates.

```yaml
Type: SwitchParameter
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

## INPUTS

### System.String

## OUTPUTS

### PSProxmox.Models.ProxmoxTurnKeyTemplate

## NOTES

## RELATED LINKS
