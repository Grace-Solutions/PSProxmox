@{
    ModuleVersion = '2025.05.11.1030'
    GUID = 'd24f0894-3d0c-4ef1-a41e-b273c3db86ad'
    Author = 'PSProxmox Team'
    CompanyName = 'PSProxmox'
    Copyright = 'Copyright © 2023'
    Description = 'PowerShell module for managing Proxmox VE clusters'
    PowerShellVersion = '5.1'
    DotNetFrameworkVersion = '4.7.2'
    CompatiblePSEditions = @('Desktop', 'Core')
    # Assemblies that must be loaded prior to importing this module
    RequiredAssemblies = @('bin\PSProxmox.dll', 'lib\Newtonsoft.Json.dll')
    # Root module
    RootModule = 'bin\PSProxmox.dll'
    # Script files (.ps1) that are run in the caller's environment prior to importing this module
    ScriptsToProcess = @()
    # Type files (.ps1xml) to be loaded when importing this module
    TypesToProcess = @()
    # Format files (.ps1xml) to be loaded when importing this module
    FormatsToProcess = @()
    FunctionsToExport = @()
    CmdletsToExport = @(
        # Session Management
        'Connect-ProxmoxServer',
        'Disconnect-ProxmoxServer',
        'Test-ProxmoxConnection',

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
        'Clear-ProxmoxIPPool',

        # Cloud Image Management
        'Get-ProxmoxCloudImage',
        'Save-ProxmoxCloudImage',
        'Invoke-ProxmoxCloudImageCustomization',
        'New-ProxmoxCloudImageTemplate',
        'Set-ProxmoxVMCloudInit',

        # LXC Container Management
        'Get-ProxmoxContainer',
        'New-ProxmoxContainer',
        'New-ProxmoxContainerBuilder',
        'Remove-ProxmoxContainer',
        'Start-ProxmoxContainer',
        'Stop-ProxmoxContainer',
        'Restart-ProxmoxContainer',

        # TurnKey Template Management
        'Get-ProxmoxTurnKeyTemplate',
        'Save-ProxmoxTurnKeyTemplate',
        'New-ProxmoxContainerFromTurnKey'
    )
    VariablesToExport = @()
    AliasesToExport = @()
    PrivateData = @{
        PSData = @{
            Tags = @('Proxmox', 'VirtualMachine', 'Cluster', 'Management')
            LicenseUri = 'https://github.com/Grace-Solutions/PSProxmox/blob/main/LICENSE'
            ProjectUri = 'https://github.com/Grace-Solutions/PSProxmox'
            ReleaseNotes = 'Added LXC container and TurnKey template support. Fixed cloud image functionality and added pipeline support. See https://github.com/Grace-Solutions/PSProxmox/releases/tag/v2025.05.11.1030'
        }
    }
}




























