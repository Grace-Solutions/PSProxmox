@{
    RootModule = 'PSProxmox.dll'
    ModuleVersion = '2023.04.28.1324'
    GUID = 'd24f0894-3d0c-4ef1-a41e-b273c3db86ad'
    Author = 'PSProxmox Team'
    CompanyName = 'PSProxmox'
    Copyright = 'Copyright © 2023'
    Description = 'PowerShell module for managing Proxmox VE clusters'
    PowerShellVersion = '5.1'
    DotNetFrameworkVersion = '4.7.2'
    CompatiblePSEditions = @('Desktop', 'Core')
    FunctionsToExport = @()
    CmdletsToExport = @(
        # Session Management
        'Connect-ProxmoxServer',
        'Disconnect-ProxmoxServer',

        # Node and VM Management
        'Get-ProxmoxNode',
        'Get-ProxmoxVM',
        'New-ProxmoxVM',
        'Remove-ProxmoxVM',
        'Start-ProxmoxVM',
        'Stop-ProxmoxVM',
        'Restart-ProxmoxVM',

        # Storage Management
        'Get-ProxmoxStorage',
        'New-ProxmoxStorage',
        'Remove-ProxmoxStorage',

        # Network Management
        'Get-ProxmoxNetwork',
        'New-ProxmoxNetwork',
        'Remove-ProxmoxNetwork',

        # User and Role Management
        'Get-ProxmoxUser',
        'New-ProxmoxUser',
        'Remove-ProxmoxUser',
        'Get-ProxmoxRole',
        'New-ProxmoxRole',
        'Remove-ProxmoxRole',

        # SDN Management
        'Get-ProxmoxSDNZone',
        'New-ProxmoxSDNZone',
        'Remove-ProxmoxSDNZone',
        'Get-ProxmoxSDNVnet',
        'New-ProxmoxSDNVnet',
        'Remove-ProxmoxSDNVnet',

        # Cluster Management
        'Get-ProxmoxCluster',
        'Join-ProxmoxCluster',
        'Leave-ProxmoxCluster',
        'New-ProxmoxClusterBackup',
        'Restore-ProxmoxClusterBackup',

        # Template Management
        'Get-ProxmoxVMTemplate',
        'New-ProxmoxVMTemplate',
        'Remove-ProxmoxVMTemplate',
        'New-ProxmoxVMFromTemplate',

        # IP Management
        'New-ProxmoxIPPool',
        'Get-ProxmoxIPPool',
        'Clear-ProxmoxIPPool'
    )
    VariablesToExport = @()
    AliasesToExport = @()
    PrivateData = @{
        PSData = @{
            Tags = @('Proxmox', 'VirtualMachine', 'Cluster', 'Management')
            LicenseUri = 'https://github.com/freedbygrace/PSProxmox/blob/main/LICENSE'
            ProjectUri = 'https://github.com/freedbygrace/PSProxmox'
            ReleaseNotes = 'Initial release of PSProxmox module'
        }
    }
}
