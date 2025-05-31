# Changelog

All notable changes to the PSProxmox module will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2025.05.30.2323] - 2025-05-30

### Added
- **Performance Optimization**: Added `-IncludeGuestAgent` parameter to Get-ProxmoxVM cmdlet
  - Guest agent data retrieval is now optional for improved query performance
  - Default behavior: Fast queries without guest agent data
  - Use `-IncludeGuestAgent` switch when detailed network information is needed
  - Significant performance improvement for normal VM queries

### Changed
- **Get-ProxmoxVM Behavior**: Guest agent information is no longer fetched by default
  - Breaking change: Existing scripts that rely on guest agent data must add `-IncludeGuestAgent` parameter
  - Performance benefit: Default queries are now much faster
- **Documentation Updates**: All guest agent examples updated to use new parameter

### Fixed
- **Binary Module Structure**: Removed unnecessary PSProxmox.psd1 file from bin output directory
- **Project Configuration**: Cleaned up project file references for proper binary module packaging

## [2025.05.30.1740] - 2025-05-30

### Added
- **VM Guest Agent Support**: Enhanced Get-ProxmoxVM cmdlet with comprehensive guest agent data retrieval
  - Added ProxmoxVMGuestAgent model with network interface information
  - Added IPv4 and IPv6 address arrays for each network interface
  - Added guest agent status and availability checking
  - Robust error handling for VMs without guest agent installed
- **Enhanced VM Models**: Added GuestAgent property to ProxmoxVM model for complete VM information
- **Network Interface Details**: Guest agent data now includes detailed network configuration from within the VM

### Fixed
- **Complete Compilation Error Resolution**: Fixed all 41 pre-existing compilation errors (100% success rate)
  - Fixed API pattern inconsistencies across all cmdlets
  - Replaced incorrect `Connection.GetJson()` calls with proper `ProxmoxApiClient` usage
  - Fixed parameter type conversions from `Dictionary<string, object>` to `Dictionary<string, string>`
  - Eliminated problematic `dynamic` types with proper JSON handling using `JObject`/`JArray`
  - Added missing using statements for required namespaces
  - Fixed string-to-long conversion issues with proper parsing
- **Warning Resolution**: Addressed all compilation warnings for clean builds
  - Fixed System.Management.Automation version mismatch warnings
  - Resolved CS0108 hidden inherited member warnings with explicit `new` keywords
- **API Client Standardization**: Standardized all cmdlets to use consistent API patterns
  - Fixed container management cmdlets (Start, Stop, Restart, Remove, New)
  - Fixed TurnKey template cmdlets (Get, Save, NewContainerFromTurnKey)
  - Fixed cluster backup cmdlets (New, Restore)
  - Fixed VM SMBIOS cmdlets (Get, Set)
- **Build System**: Fixed missing PSProxmox.psd1 file dependency for successful compilation

### Changed
- **Improved Error Handling**: Enhanced error handling throughout the codebase for better reliability
- **Type Safety**: Improved type safety across all API interactions
- **Code Quality**: Applied modern C# patterns and best practices consistently

## [2025.04.28.2035] - 2025-04-28

### Added
- Comprehensive documentation in the Documentation folder
- Example scripts for common tasks
- Installation guide and usage guides
- Build and installation scripts in the Scripts folder

### Changed
- Reorganized project structure to separate source code from module files
- Updated module manifest to use NestedModules for loading the DLL
- Improved build process with dedicated build scripts
- Enhanced installation process with multiple installation options

### Fixed
- Fixed dependency loading by including Newtonsoft.Json.dll in the module
- Fixed Connect-ProxmoxServer to return ProxmoxConnection instead of ProxmoxConnectionInfo
- Fixed module loading issues by using a PSM1 file as the root module
- Fixed type mismatch between ProxmoxConnectionInfo and ProxmoxConnection

## [2023.04.28.1324] - 2023-04-28

### Added

- Initial release of PSProxmox module
- Session management with Connect-ProxmoxServer and Disconnect-ProxmoxServer
- VM management with Get-ProxmoxVM, New-ProxmoxVM, Remove-ProxmoxVM, Start-ProxmoxVM, Stop-ProxmoxVM, Restart-ProxmoxVM
- VM builder pattern for complex VM configurations
- Storage management with Get-ProxmoxStorage, New-ProxmoxStorage, Remove-ProxmoxStorage
- Network management with Get-ProxmoxNetwork, New-ProxmoxNetwork, Remove-ProxmoxNetwork
- User and role management with Get-ProxmoxUser, New-ProxmoxUser, Remove-ProxmoxUser, Get-ProxmoxRole, New-ProxmoxRole, Remove-ProxmoxRole
- SDN management with Get-ProxmoxSDNZone, New-ProxmoxSDNZone, Remove-ProxmoxSDNZone, Get-ProxmoxSDNVnet, New-ProxmoxSDNVnet, Remove-ProxmoxSDNVnet
- Cluster management with Get-ProxmoxCluster, Join-ProxmoxCluster, Leave-ProxmoxCluster
- Cluster backup management with New-ProxmoxClusterBackup, Get-ProxmoxClusterBackup, Restore-ProxmoxClusterBackup
- Template management with New-ProxmoxVMTemplate, Get-ProxmoxVMTemplate, Remove-ProxmoxVMTemplate, New-ProxmoxVMFromTemplate
- IP management with New-ProxmoxIPPool, Get-ProxmoxIPPool, Clear-ProxmoxIPPool
- Comprehensive unit tests for all components
- Detailed documentation and examples

### Changed

- N/A (initial release)

### Fixed

- N/A (initial release)
