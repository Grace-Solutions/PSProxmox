# PSProxmox Module

This directory contains the PowerShell module files for PSProxmox.

## Structure

- **PSProxmox.psd1**: The module manifest file
- **bin/**: Contains the compiled DLL

## Building the Module

The module is built using the build script in the Scripts directory:

```powershell
.\Scripts\build.ps1
```

This will compile the C# code from the PSProxmox directory and place the DLL in the bin directory.

## Installing the Module

You can install the module using the installation script in the Scripts directory:

```powershell
.\Scripts\Install-PSProxmox.ps1
```

This will copy the module files to your PowerShell modules directory.

## Manual Installation

You can manually install the module by copying the files to your PowerShell modules directory:

For Windows PowerShell:
```powershell
Copy-Item -Path .\Module\* -Destination "$env:USERPROFILE\Documents\WindowsPowerShell\Modules\PSProxmox" -Recurse -Force
```

For PowerShell Core:
```powershell
Copy-Item -Path .\Module\* -Destination "$env:USERPROFILE\Documents\PowerShell\Modules\PSProxmox" -Recurse -Force
```

## Documentation

For detailed documentation, see the [Documentation](../Documentation) directory.


