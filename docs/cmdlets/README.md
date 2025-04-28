# PSProxmox Cmdlets

This directory contains documentation for all cmdlets in the PSProxmox module.

## Cmdlet Categories

- [Session Management](#session-management)
- [VM Management](#vm-management)
- [Storage Management](#storage-management)
- [Network Management](#network-management)
- [User and Role Management](#user-and-role-management)
- [SDN Management](#sdn-management)
- [Cluster Management](#cluster-management)
- [Template Management](#template-management)
- [IP Management](#ip-management)

## Session Management

- [Connect-ProxmoxServer](Connect-ProxmoxServer.md) - Connects to a Proxmox VE server
- [Disconnect-ProxmoxServer](Disconnect-ProxmoxServer.md) - Disconnects from a Proxmox VE server

## VM Management

- [Get-ProxmoxVM](Get-ProxmoxVM.md) - Gets virtual machines from Proxmox VE
- [New-ProxmoxVM](New-ProxmoxVM.md) - Creates a new virtual machine in Proxmox VE
- [Remove-ProxmoxVM](Remove-ProxmoxVM.md) - Removes a virtual machine from Proxmox VE
- [Start-ProxmoxVM](Start-ProxmoxVM.md) - Starts a virtual machine in Proxmox VE
- [Stop-ProxmoxVM](Stop-ProxmoxVM.md) - Stops a virtual machine in Proxmox VE
- [Restart-ProxmoxVM](Restart-ProxmoxVM.md) - Restarts a virtual machine in Proxmox VE

## Storage Management

- [Get-ProxmoxStorage](Get-ProxmoxStorage.md) - Gets storage from Proxmox VE
- [New-ProxmoxStorage](New-ProxmoxStorage.md) - Creates a new storage in Proxmox VE
- [Remove-ProxmoxStorage](Remove-ProxmoxStorage.md) - Removes a storage from Proxmox VE

## Network Management

- [Get-ProxmoxNetwork](Get-ProxmoxNetwork.md) - Gets network interfaces from Proxmox VE
- [New-ProxmoxNetwork](New-ProxmoxNetwork.md) - Creates a new network interface in Proxmox VE
- [Remove-ProxmoxNetwork](Remove-ProxmoxNetwork.md) - Removes a network interface from Proxmox VE

## User and Role Management

- [Get-ProxmoxUser](Get-ProxmoxUser.md) - Gets users from Proxmox VE
- [New-ProxmoxUser](New-ProxmoxUser.md) - Creates a new user in Proxmox VE
- [Remove-ProxmoxUser](Remove-ProxmoxUser.md) - Removes a user from Proxmox VE
- [Get-ProxmoxRole](Get-ProxmoxRole.md) - Gets roles from Proxmox VE
- [New-ProxmoxRole](New-ProxmoxRole.md) - Creates a new role in Proxmox VE
- [Remove-ProxmoxRole](Remove-ProxmoxRole.md) - Removes a role from Proxmox VE

## SDN Management

- [Get-ProxmoxSDNZone](Get-ProxmoxSDNZone.md) - Gets SDN zones from Proxmox VE
- [New-ProxmoxSDNZone](New-ProxmoxSDNZone.md) - Creates a new SDN zone in Proxmox VE
- [Remove-ProxmoxSDNZone](Remove-ProxmoxSDNZone.md) - Removes an SDN zone from Proxmox VE
- [Get-ProxmoxSDNVnet](Get-ProxmoxSDNVnet.md) - Gets SDN VNets from Proxmox VE
- [New-ProxmoxSDNVnet](New-ProxmoxSDNVnet.md) - Creates a new SDN VNet in Proxmox VE
- [Remove-ProxmoxSDNVnet](Remove-ProxmoxSDNVnet.md) - Removes an SDN VNet from Proxmox VE

## Cluster Management

- [Get-ProxmoxCluster](Get-ProxmoxCluster.md) - Gets cluster information from Proxmox VE
- [Join-ProxmoxCluster](Join-ProxmoxCluster.md) - Joins a node to a Proxmox VE cluster
- [Leave-ProxmoxCluster](Leave-ProxmoxCluster.md) - Removes a node from a Proxmox VE cluster
- [Get-ProxmoxClusterBackup](Get-ProxmoxClusterBackup.md) - Gets cluster backups from Proxmox VE
- [New-ProxmoxClusterBackup](New-ProxmoxClusterBackup.md) - Creates a new cluster backup in Proxmox VE
- [Restore-ProxmoxClusterBackup](Restore-ProxmoxClusterBackup.md) - Restores a cluster backup in Proxmox VE

## Template Management

- [Get-ProxmoxVMTemplate](Get-ProxmoxVMTemplate.md) - Gets VM templates from Proxmox VE
- [New-ProxmoxVMTemplate](New-ProxmoxVMTemplate.md) - Creates a new VM template in Proxmox VE
- [Remove-ProxmoxVMTemplate](Remove-ProxmoxVMTemplate.md) - Removes a VM template from Proxmox VE
- [New-ProxmoxVMFromTemplate](New-ProxmoxVMFromTemplate.md) - Creates a new VM from a template in Proxmox VE

## IP Management

- [New-ProxmoxIPPool](New-ProxmoxIPPool.md) - Creates a new IP pool
- [Get-ProxmoxIPPool](Get-ProxmoxIPPool.md) - Gets IP pools
- [Clear-ProxmoxIPPool](Clear-ProxmoxIPPool.md) - Clears an IP pool
