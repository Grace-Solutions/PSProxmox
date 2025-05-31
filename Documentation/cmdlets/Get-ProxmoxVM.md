# Get-ProxmoxVM

Gets virtual machines from Proxmox VE with comprehensive information including guest agent data.

## Syntax

```powershell
Get-ProxmoxVM
   [-Connection <ProxmoxConnection>]
   [-Node <String>]
   [-VMID <Int32>]
   [-Name <String>]
   [-UseRegex]
   [-RawJson]
   [-IncludeGuestAgent]
   [<CommonParameters>]
```

## Description

The `Get-ProxmoxVM` cmdlet retrieves virtual machines from Proxmox VE. You can retrieve all VMs, VMs on a specific node, or a specific VM by ID. Use the `-IncludeGuestAgent` parameter to fetch guest agent information, which provides detailed network interface information from within the guest operating system. Note that including guest agent data may slow down queries as it requires additional API calls.

## Parameters

### -Connection

The connection to the Proxmox VE server.

```yaml
Type: ProxmoxConnection
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Node

The node to get VMs from. If not specified, VMs from all nodes will be returned.

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

### -VMID

The ID of the VM to get. If not specified, all VMs will be returned.

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

### -Name

The name of the virtual machine to retrieve. Supports wildcards and regex when used with -UseRegex.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 2
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

### -RawJson

Whether to return the raw JSON response instead of parsed objects.

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

### -IncludeGuestAgent

Whether to include guest agent information. This may slow down the query as it requires additional API calls.

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

### PSProxmox.Models.ProxmoxVM

### System.String

## Notes

- This cmdlet requires a connection to a Proxmox VE server. Use `Connect-ProxmoxServer` to establish a connection.
- If no connection is specified, the cmdlet will use the default connection.
- If the `-RawJson` parameter is specified, the raw JSON response will be returned instead of parsed objects.
- If the `-VMID` parameter is specified and the VM is not found, an error will be thrown.
- The `-Name` parameter supports wildcards (e.g., "web*") by default.
- Use the `-UseRegex` parameter with `-Name` to filter using regular expressions (e.g., "^web[0-9]+$").

## Examples

### Example 1: Get all VMs

```powershell
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$vms = Get-ProxmoxVM
```

This example gets all VMs from all nodes.

### Example 2: Get VMs on a specific node

```powershell
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$vms = Get-ProxmoxVM -Node "pve1"
```

This example gets all VMs on the node "pve1".

### Example 3: Get a specific VM by ID

```powershell
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$vm = Get-ProxmoxVM -VMID 100
```

This example gets the VM with ID 100.

### Example 4: Get VMs by name using wildcards

```powershell
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$vms = Get-ProxmoxVM -Name "web*"
```

This example gets all VMs with names starting with "web".

### Example 5: Get VMs by name using regex

```powershell
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$vms = Get-ProxmoxVM -Name "^web[0-9]+$" -UseRegex
```

This example gets all VMs with names matching the pattern "web" followed by one or more digits.

### Example 6: Get the raw JSON response

```powershell
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$json = Get-ProxmoxVM -RawJson
```

This example gets the raw JSON response for all VMs.

### Example 7: Get VM with guest agent information

```powershell
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$vm = Get-ProxmoxVM -VMID 100 -IncludeGuestAgent

# Check if guest agent is available and running
if ($vm.GuestAgent -and $vm.GuestAgent.Status -eq "running") {
    Write-Host "Guest Agent is running"

    # Display network interfaces from guest agent
    foreach ($interface in $vm.GuestAgent.NetIf) {
        Write-Host "Interface: $($interface.Name)"
        Write-Host "  IPv4 Addresses: $($interface.IPv4Addresses -join ', ')"
        Write-Host "  IPv6 Addresses: $($interface.IPv6Addresses -join ', ')"
        Write-Host "  MAC Address: $($interface.MacAddress)"
    }
} else {
    Write-Host "Guest Agent not available or not running"
}
```

This example gets a VM with guest agent information and displays network interface details.

### Example 8: Filter VMs with active guest agents

```powershell
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$vmsWithGuestAgent = Get-ProxmoxVM -IncludeGuestAgent | Where-Object {
    $_.GuestAgent -and $_.GuestAgent.Status -eq "running"
}

foreach ($vm in $vmsWithGuestAgent) {
    Write-Host "$($vm.Name): $($vm.GuestAgent.NetIf.Count) network interfaces"
}
```

This example gets all VMs with guest agent information and filters those with active guest agents.

### Example 9: Performance comparison - with and without guest agent

```powershell
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)

# Fast query without guest agent information
Measure-Command { $vms = Get-ProxmoxVM }

# Slower query with guest agent information
Measure-Command { $vmsWithGA = Get-ProxmoxVM -IncludeGuestAgent }
```

This example demonstrates the performance difference between queries with and without guest agent information.

## Related Links

- [Connect-ProxmoxServer](Connect-ProxmoxServer.md)
- [New-ProxmoxVM](New-ProxmoxVM.md)
- [Remove-ProxmoxVM](Remove-ProxmoxVM.md)
- [Start-ProxmoxVM](Start-ProxmoxVM.md)
- [Stop-ProxmoxVM](Stop-ProxmoxVM.md)
- [Restart-ProxmoxVM](Restart-ProxmoxVM.md)
