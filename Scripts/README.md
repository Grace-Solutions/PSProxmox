# PSProxmox Scripts

This directory contains scripts for building and installing the PSProxmox module.

## Scripts

- **build.ps1**: Builds the PSProxmox module
- **Install-PSProxmox.ps1**: Installs the PSProxmox module

## Building the Module

To build the module, run the build.ps1 script:

```powershell
.\build.ps1
```

This will:
1. Compile the C# code
2. Create a versioned release in the Release folder
3. Update the Module folder with the latest build
4. Create a ZIP package for distribution

## Installing the Module

To install the module, run the Install-PSProxmox.ps1 script:

```powershell
.\Install-PSProxmox.ps1
```

This will copy the module files to your PowerShell modules directory.

## Documentation

For detailed documentation, see the [Documentation](../Documentation) directory.
