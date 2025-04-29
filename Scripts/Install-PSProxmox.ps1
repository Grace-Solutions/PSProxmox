# Install-PSProxmox.ps1
# This script installs the PSProxmox module to the user's PowerShell modules directory

# Set script root path for relative paths
$scriptRoot = $PSScriptRoot
$rootPath = Split-Path -Parent $scriptRoot

# Define the module name
$moduleName = "PSProxmox"

# Define the destination path
$modulePath = "$env:USERPROFILE\Documents\WindowsPowerShell\Modules\$moduleName"
if ($PSVersionTable.PSEdition -eq "Core") {
    $modulePath = "$env:USERPROFILE\Documents\PowerShell\Modules\$moduleName"
}

# Create the module directory if it doesn't exist
if (-not (Test-Path -Path $modulePath)) {
    New-Item -Path $modulePath -ItemType Directory -Force | Out-Null
    Write-Host "Created module directory: $modulePath"
}

# Create the bin directory if it doesn't exist
if (-not (Test-Path -Path "$modulePath\bin")) {
    New-Item -Path "$modulePath\bin" -ItemType Directory -Force | Out-Null
    Write-Host "Created bin directory: $modulePath\bin"
}

# Copy the module files
Copy-Item -Path "$rootPath\Module\PSProxmox.psd1" -Destination $modulePath -Force
Copy-Item -Path "$rootPath\Module\PSProxmox.psm1" -Destination $modulePath -Force
Copy-Item -Path "$rootPath\Module\bin\PSProxmox.dll" -Destination "$modulePath\bin" -Force
Copy-Item -Path "$rootPath\Module\bin\Newtonsoft.Json.dll" -Destination "$modulePath\bin" -Force
Copy-Item -Path "$rootPath\LICENSE" -Destination $modulePath -Force
Copy-Item -Path "$rootPath\README.md" -Destination $modulePath -Force

Write-Host "PSProxmox module has been installed to $modulePath"
Write-Host "You can now import the module using: Import-Module $moduleName"
