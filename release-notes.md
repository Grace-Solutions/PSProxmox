# PSProxmox v2025.05.10.1000 Release Notes

## New Features

### Automatic Connection Handling
- Added automatic connection handling so cmdlets use the default connection without explicitly passing it
- Added a global `DefaultProxmoxConnection` variable that stores the most recent connection
- Created a base `ProxmoxCmdlet` class that all cmdlets can inherit from
- Updated all cmdlets to use the base class and make the Connection parameter optional

### Test-ProxmoxConnection Cmdlet
- Added a new cmdlet to test if a connection is valid and active
- Added a `-Detailed` parameter to get comprehensive connection information
- The cmdlet can test either a specified connection or the default connection

### Connection Object Improvements
- Added a custom `ToString()` method to the `ProxmoxConnection` class
- The connection now displays as "Proxmox Connection: username@realm on server:port (Authenticated)"
- This makes it easier to see connection details in the console

### Documentation Updates
- Updated all documentation and examples to use the new automatic connection handling
- Added documentation for the new `Test-ProxmoxConnection` cmdlet
- Updated the README.md examples to use the default connection

## Usage Examples

**Old way (explicit connection):**
```powershell
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$vms = Get-ProxmoxVM -Connection $connection
Disconnect-ProxmoxServer -Connection $connection
```

**New way (automatic connection):**
```powershell
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)
$vms = Get-ProxmoxVM
$connection = Test-ProxmoxConnection -Detailed
Disconnect-ProxmoxServer -Connection $connection
```

## Installation
1. Download the ZIP file
2. Extract the contents to a directory in your PowerShell module path
3. Import the module using `Import-Module PSProxmox`

## Requirements
- PowerShell 5.1 or later
- Windows PowerShell or PowerShell Core
