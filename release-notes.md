# PSProxmox v2025.05.30.1740 Release Notes

## Major Improvements

### 🎉 VM Guest Agent Support
- **Enhanced Get-ProxmoxVM cmdlet** with comprehensive guest agent data retrieval
- **New ProxmoxVMGuestAgent model** with detailed network interface information
- **IPv4 and IPv6 address arrays** for each network interface detected by guest agent
- **Guest agent status checking** with robust error handling for VMs without guest agent
- **Complete VM network visibility** from within the guest operating system

### 🔧 Complete Codebase Stabilization
- **Fixed ALL 41 compilation errors** (100% success rate) that were present in the codebase
- **Eliminated ALL compilation warnings** for completely clean builds
- **Standardized API patterns** across all cmdlets for consistency and reliability
- **Enhanced type safety** throughout the entire codebase
- **Modern C# best practices** applied consistently

### 🚀 Enhanced Reliability
- **Robust error handling** for guest agent operations
- **Improved API client usage** with proper parameter handling
- **Better JSON processing** with strongly-typed objects instead of dynamic types
- **Consistent parameter validation** across all cmdlets

## Usage Examples

### 🔍 VM Guest Agent Information
```powershell
# Connect to Proxmox
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)

# Get VM with guest agent information
$vm = Get-ProxmoxVM -VMID 100

# Access guest agent data
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

# Get all VMs and filter by those with guest agent
$vmsWithGuestAgent = Get-ProxmoxVM | Where-Object {
    $_.GuestAgent -and $_.GuestAgent.Status -eq "running"
}
```

### 🔧 Clean Build and Development
```powershell
# The module now builds completely clean with no errors or warnings
dotnet build PSProxmox\PSProxmox.csproj
# Build succeeded with 0 error(s) and 0 warning(s)
```

## Installation
1. Download the ZIP file
2. Extract the contents to a directory in your PowerShell module path
3. Import the module using `Import-Module PSProxmox`

## Requirements
- PowerShell 5.1 or later
- Windows PowerShell or PowerShell Core
