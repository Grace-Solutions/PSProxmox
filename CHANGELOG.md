# Changelog

All notable changes to the PSProxmox module will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
