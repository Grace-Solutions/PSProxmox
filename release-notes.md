# PSProxmox v2025.05.09.1246 Release Notes

## New Features

### VM Creation Enhancements
- Added padded counter functionality for multi-VM creation (e.g., "Prefix-00001")
- Added AutoSMBIOS capability parameters for setting realistic hardware information
- Added Microsoft VMBIOS profile for SMBIOS settings
- Enhanced existing manufacturer profiles with more business-class workstations and servers
- Improved serial number generation for major non-VM vendors

### SMBIOS Improvements
- Only generate random UUIDs if one is not already present
- Added more realistic hardware information for Dell, HP, Lenovo, and Microsoft devices
- Added support for both server and workstation profiles for each manufacturer

## Documentation
- Added comprehensive documentation for the new features
- Added example scripts for using padded counters and SMBIOS settings
- Updated existing documentation to reflect the new capabilities

## Bug Fixes
- Fixed module loading issues by using PSM1 file and NestedModules
- Fixed type mismatch between ProxmoxConnectionInfo and ProxmoxConnection classes

## Installation
1. Download the ZIP file
2. Extract the contents to a directory in your PowerShell module path
3. Import the module using `Import-Module PSProxmox`

## Requirements
- PowerShell 5.1 or later
- Windows PowerShell or PowerShell Core
