# Install-PSProxmoxFromRelease.ps1
# This script installs the PSProxmox module from the latest release to the user's PowerShell modules directory

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

# Get the latest release
$releaseBaseDir = "$rootPath\Release"
$latestRelease = Get-ChildItem -Path $releaseBaseDir -Directory | Sort-Object Name -Descending | Select-Object -First 1
if (-not $latestRelease) {
    Write-Error "No release found in $releaseBaseDir"
    exit 1
}
$releaseDir = $latestRelease.FullName
Write-Host "Installing from release: $($latestRelease.Name)"

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
Copy-Item -Path "$releaseDir\PSProxmox.psd1" -Destination $modulePath -Force
Copy-Item -Path "$releaseDir\PSProxmox.psm1" -Destination $modulePath -Force
Copy-Item -Path "$releaseDir\bin\PSProxmox.dll" -Destination "$modulePath\bin" -Force
Copy-Item -Path "$releaseDir\bin\Newtonsoft.Json.dll" -Destination "$modulePath\bin" -Force
Copy-Item -Path "$rootPath\LICENSE" -Destination $modulePath -Force
Copy-Item -Path "$rootPath\README.md" -Destination $modulePath -Force

Write-Host "PSProxmox module has been installed to $modulePath"
Write-Host "You can now import the module using: Import-Module $moduleName"
