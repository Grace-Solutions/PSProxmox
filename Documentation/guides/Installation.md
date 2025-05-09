# Installation Guide

This guide explains how to install the PSProxmox module.

## Prerequisites

- PowerShell 5.1 or later
- Windows PowerShell or PowerShell Core
- .NET Framework 4.7.2 or later (for Windows PowerShell)
- .NET Core 2.0 or later (for PowerShell Core)

## Installation Methods

### Method 1: Install from PowerShell Gallery (Recommended)

The PSProxmox module is available on the PowerShell Gallery. To install it, run the following command:

```powershell
Install-Module -Name PSProxmox -Scope CurrentUser
```

If you want to install it for all users on the system, run the following command with administrative privileges:

```powershell
Install-Module -Name PSProxmox -Scope AllUsers
```

### Method 2: Install from GitHub Release

You can also install the PSProxmox module directly from a GitHub release:

1. Download the latest release from the [GitHub repository](https://github.com/Grace-Solutions/PSProxmox/releases)
2. Extract the ZIP file
3. Run the `Install-PSProxmox.ps1` script included in the package:

```powershell
.\Install-PSProxmox.ps1
```

### Method 3: Build and Install from Source

You can build and install the PSProxmox module from source:

1. Clone the repository:
```powershell
git clone https://github.com/Grace-Solutions/PSProxmox.git
cd PSProxmox
```

2. Build the module:
```powershell
.\Scripts\build.ps1
```

3. Install the module:
```powershell
.\Scripts\Install-PSProxmox.ps1
```

### Method 4: Manual Installation

You can manually install the PSProxmox module by copying the files to your PowerShell modules directory:

1. Download the latest release from the [GitHub repository](https://github.com/Grace-Solutions/PSProxmox/releases)
2. Extract the ZIP file
3. Copy the extracted files to a folder named "PSProxmox" in your PowerShell modules directory:

For Windows PowerShell:
```powershell
Copy-Item -Path .\* -Destination "$env:USERPROFILE\Documents\WindowsPowerShell\Modules\PSProxmox" -Recurse -Force
```

For PowerShell Core:
```powershell
Copy-Item -Path .\* -Destination "$env:USERPROFILE\Documents\PowerShell\Modules\PSProxmox" -Recurse -Force
```

## Verifying the Installation

To verify that the PSProxmox module is installed correctly, run the following command:

```powershell
Get-Module -Name PSProxmox -ListAvailable
```

You should see output similar to the following:

```
    Directory: C:\Users\username\Documents\WindowsPowerShell\Modules

ModuleType Version    Name                                ExportedCommands
---------- -------    ----                                ----------------
Binary     2023.04... PSProxmox                           {Clear-ProxmoxIPPool, Connect-ProxmoxServer, Disconnect-ProxmoxServer, Get-ProxmoxCluster...}
```

## Importing the Module

To import the PSProxmox module into your PowerShell session, run the following command:

```powershell
Import-Module PSProxmox
```

You can verify that the module is imported correctly by running:

```powershell
Get-Command -Module PSProxmox
```

This will list all the cmdlets available in the PSProxmox module.

## Updating the Module

To update the PSProxmox module to the latest version, run the following command:

```powershell
Update-Module -Name PSProxmox
```

## Uninstalling the Module

To uninstall the PSProxmox module, run the following command:

```powershell
Uninstall-Module -Name PSProxmox
```

## Next Steps

Now that you have installed the PSProxmox module, you can proceed to the [Connection Guide](Connection.md) to learn how to connect to a Proxmox VE server.
