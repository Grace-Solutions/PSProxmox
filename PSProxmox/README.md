# PSProxmox Source Code

This directory contains the C# source code for the PSProxmox module.

## Structure

- **Client/**: Contains the API client code
- **Cmdlets/**: Contains the PowerShell cmdlet implementations
- **IPAM/**: Contains the IP address management code
- **Models/**: Contains the data models
- **Session/**: Contains the session management code
- **Templates/**: Contains the template management code
- **Utilities/**: Contains utility functions
- **bin/**: Contains the compiled DLL (created during build)
- **obj/**: Contains intermediate build files (created during build)
- **PSProxmox.Main.csproj**: The main project file
- **PSProxmox.Main.sln**: The solution file

## Building

The source code is built using the build script in the Scripts directory:

```powershell
.\Scripts\build.ps1
```

This will compile the C# code and place the DLL in the Module\bin directory.

## Development

### Adding a New Cmdlet

1. Create a new file in the Cmdlets directory
2. Implement the cmdlet class
3. Add the cmdlet to the CmdletsToExport array in the Module\PSProxmox.psd1 file

### Adding a New Model

1. Create a new file in the Models directory
2. Implement the model class

### Adding a New API Client Method

1. Add the method to the ProxmoxApiClient class in the Client directory

## Documentation

For detailed documentation, see the [Documentation](../Documentation) directory.
