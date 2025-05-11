# Get-ProxmoxContainer

## SYNOPSIS
Gets Proxmox LXC containers.

## SYNTAX

```powershell
Get-ProxmoxContainer [[-CTID] <Int32>] [[-Node] <String>] [[-Name] <String>] [<CommonParameters>]
```

## DESCRIPTION
Gets Proxmox LXC containers from a Proxmox server.

If no parameters are specified, all containers are returned.
If a CTID is specified, only that container is returned.
If a node is specified, only containers on that node are returned.
If a name is specified, only containers with that name are returned (supports wildcards and regex).

## EXAMPLES

### Example 1: Get all containers
```powershell
Get-ProxmoxContainer
```

Gets all containers from the Proxmox server.

### Example 2: Get a specific container
```powershell
Get-ProxmoxContainer -CTID 100
```

Gets the container with CTID 100.

### Example 3: Get all containers on a specific node
```powershell
Get-ProxmoxContainer -Node "pve1"
```

Gets all containers on node pve1.

### Example 4: Get containers by name using wildcards
```powershell
Get-ProxmoxContainer -Name "web*"
```

Gets all containers with names starting with "web" (wildcard).

### Example 5: Get containers by name using regex
```powershell
Get-ProxmoxContainer -Name "^web\d+$"
```

Gets all containers with names matching the regex pattern "^web\d+$".

## PARAMETERS

### -CTID
The container ID.

```yaml
Type: Int32
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: True (ByValue, ByPropertyName)
Accept wildcard characters: False
```

### -Node
The node name.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 1
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -Name
The container name (supports wildcards and regex).

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 2
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: True
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.Int32
### System.String

## OUTPUTS

### PSProxmox.Models.ProxmoxContainer

## NOTES

## RELATED LINKS
