# VM-GuestAgent-Examples.ps1
# This script demonstrates how to use the VM Guest Agent functionality in PSProxmox

# Import the PSProxmox module
Import-Module PSProxmox

# Connect to the Proxmox VE server
$credential = Get-Credential -Message "Enter your Proxmox VE credentials"
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential $credential -Realm "pam"

Write-Host "=== PSProxmox VM Guest Agent Examples ===" -ForegroundColor Green

# Example 1: Get a specific VM with guest agent information
Write-Host "`n1. Getting VM with Guest Agent Information" -ForegroundColor Yellow
$vmid = 100  # Change this to your VM ID
$vm = Get-ProxmoxVM -VMID $vmid

if ($vm) {
    Write-Host "VM: $($vm.Name) (ID: $($vm.VMID))" -ForegroundColor Cyan
    
    if ($vm.GuestAgent) {
        Write-Host "Guest Agent Status: $($vm.GuestAgent.Status)" -ForegroundColor Green
        
        if ($vm.GuestAgent.Status -eq "running" -and $vm.GuestAgent.NetIf) {
            Write-Host "Network Interfaces from Guest Agent:" -ForegroundColor Green
            
            foreach ($interface in $vm.GuestAgent.NetIf) {
                Write-Host "  Interface: $($interface.Name)" -ForegroundColor White
                Write-Host "    MAC Address: $($interface.MacAddress)" -ForegroundColor Gray
                
                if ($interface.IPv4Addresses -and $interface.IPv4Addresses.Count -gt 0) {
                    Write-Host "    IPv4 Addresses: $($interface.IPv4Addresses -join ', ')" -ForegroundColor Gray
                }
                
                if ($interface.IPv6Addresses -and $interface.IPv6Addresses.Count -gt 0) {
                    Write-Host "    IPv6 Addresses: $($interface.IPv6Addresses -join ', ')" -ForegroundColor Gray
                }
                Write-Host ""
            }
        } else {
            Write-Host "Guest Agent is not running or no network interfaces available" -ForegroundColor Yellow
        }
    } else {
        Write-Host "Guest Agent information not available" -ForegroundColor Red
    }
} else {
    Write-Host "VM with ID $vmid not found" -ForegroundColor Red
}

# Example 2: Get all VMs and show guest agent status
Write-Host "`n2. Guest Agent Status for All VMs" -ForegroundColor Yellow
$allVMs = Get-ProxmoxVM

Write-Host "VM Guest Agent Status Summary:" -ForegroundColor Cyan
$guestAgentStats = @{
    Running = 0
    NotRunning = 0
    NotAvailable = 0
}

foreach ($vm in $allVMs) {
    $status = "Not Available"
    $color = "Red"
    
    if ($vm.GuestAgent) {
        if ($vm.GuestAgent.Status -eq "running") {
            $status = "Running"
            $color = "Green"
            $guestAgentStats.Running++
        } else {
            $status = "Not Running"
            $color = "Yellow"
            $guestAgentStats.NotRunning++
        }
    } else {
        $guestAgentStats.NotAvailable++
    }
    
    Write-Host "  $($vm.Name) (ID: $($vm.VMID)): $status" -ForegroundColor $color
}

Write-Host "`nSummary:" -ForegroundColor Cyan
Write-Host "  Running: $($guestAgentStats.Running)" -ForegroundColor Green
Write-Host "  Not Running: $($guestAgentStats.NotRunning)" -ForegroundColor Yellow
Write-Host "  Not Available: $($guestAgentStats.NotAvailable)" -ForegroundColor Red

# Example 3: Filter VMs with active guest agents
Write-Host "`n3. VMs with Active Guest Agents" -ForegroundColor Yellow
$vmsWithActiveGA = $allVMs | Where-Object { 
    $_.GuestAgent -and $_.GuestAgent.Status -eq "running" 
}

if ($vmsWithActiveGA.Count -gt 0) {
    Write-Host "VMs with active Guest Agents:" -ForegroundColor Green
    
    foreach ($vm in $vmsWithActiveGA) {
        Write-Host "  $($vm.Name) (ID: $($vm.VMID))" -ForegroundColor White
        
        if ($vm.GuestAgent.NetIf) {
            $totalIPs = ($vm.GuestAgent.NetIf | ForEach-Object { 
                $_.IPv4Addresses.Count + $_.IPv6Addresses.Count 
            } | Measure-Object -Sum).Sum
            
            Write-Host "    Network Interfaces: $($vm.GuestAgent.NetIf.Count)" -ForegroundColor Gray
            Write-Host "    Total IP Addresses: $totalIPs" -ForegroundColor Gray
        }
    }
} else {
    Write-Host "No VMs found with active Guest Agents" -ForegroundColor Yellow
}

# Example 4: Export guest agent network information to CSV
Write-Host "`n4. Exporting Guest Agent Network Information" -ForegroundColor Yellow
$networkData = @()

foreach ($vm in $vmsWithActiveGA) {
    if ($vm.GuestAgent.NetIf) {
        foreach ($interface in $vm.GuestAgent.NetIf) {
            foreach ($ipv4 in $interface.IPv4Addresses) {
                $networkData += [PSCustomObject]@{
                    VMName = $vm.Name
                    VMID = $vm.VMID
                    InterfaceName = $interface.Name
                    MACAddress = $interface.MacAddress
                    IPAddress = $ipv4
                    IPVersion = "IPv4"
                }
            }
            
            foreach ($ipv6 in $interface.IPv6Addresses) {
                $networkData += [PSCustomObject]@{
                    VMName = $vm.Name
                    VMID = $vm.VMID
                    InterfaceName = $interface.Name
                    MACAddress = $interface.MacAddress
                    IPAddress = $ipv6
                    IPVersion = "IPv6"
                }
            }
        }
    }
}

if ($networkData.Count -gt 0) {
    $csvPath = "VM-GuestAgent-NetworkInfo.csv"
    $networkData | Export-Csv -Path $csvPath -NoTypeInformation
    Write-Host "Network information exported to: $csvPath" -ForegroundColor Green
    Write-Host "Total network entries: $($networkData.Count)" -ForegroundColor Cyan
} else {
    Write-Host "No network information available to export" -ForegroundColor Yellow
}

# Example 5: Find VMs by IP address using guest agent data
Write-Host "`n5. Find VM by IP Address" -ForegroundColor Yellow
$searchIP = "192.168.1.100"  # Change this to the IP you're looking for
Write-Host "Searching for VMs with IP address: $searchIP" -ForegroundColor Cyan

$foundVMs = $vmsWithActiveGA | Where-Object {
    $vm = $_
    $found = $false
    
    if ($vm.GuestAgent.NetIf) {
        foreach ($interface in $vm.GuestAgent.NetIf) {
            if ($interface.IPv4Addresses -contains $searchIP -or 
                $interface.IPv6Addresses -contains $searchIP) {
                $found = $true
                break
            }
        }
    }
    
    return $found
}

if ($foundVMs.Count -gt 0) {
    Write-Host "Found VMs with IP $searchIP" -ForegroundColor Green
    foreach ($vm in $foundVMs) {
        Write-Host "  $($vm.Name) (ID: $($vm.VMID))" -ForegroundColor White
    }
} else {
    Write-Host "No VMs found with IP address $searchIP" -ForegroundColor Yellow
}

Write-Host "`n=== Examples Complete ===" -ForegroundColor Green

# Disconnect from the server when done
$connection = Test-ProxmoxConnection -Detailed
Disconnect-ProxmoxServer -Connection $connection
