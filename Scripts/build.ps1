# build.ps1
# This script builds the PSProxmox module and creates a release package

# Set script root path for relative paths
$scriptRoot = $PSScriptRoot
$rootPath = Split-Path -Parent $scriptRoot

# Set the version based on the current date and time
$version = Get-Date -Format "yyyy.MM.dd.HHmm"
Write-Host "Building PSProxmox version $version"

# Update the module version in the PSD1 file
$psd1Path = "$rootPath\Module\PSProxmox.psd1"
$psd1Content = Get-Content -Path $psd1Path -Raw
$psd1Content = $psd1Content -replace "ModuleVersion = '.*'", "ModuleVersion = '$version'"
Set-Content -Path $psd1Path -Value $psd1Content

# Create the release directory structure
$releaseBaseDir = "$rootPath\Release"
$releaseVersionDir = "$releaseBaseDir\$version"
$releaseBinDir = "$releaseVersionDir\bin"

# Create directories if they don't exist
if (-not (Test-Path -Path $releaseBaseDir)) {
    New-Item -Path $releaseBaseDir -ItemType Directory -Force | Out-Null
}
if (-not (Test-Path -Path $releaseVersionDir)) {
    New-Item -Path $releaseVersionDir -ItemType Directory -Force | Out-Null
    Write-Host "Created release directory: $releaseVersionDir"
}
if (-not (Test-Path -Path $releaseBinDir)) {
    New-Item -Path $releaseBinDir -ItemType Directory -Force | Out-Null
    Write-Host "Created bin directory: $releaseBinDir"
}

# Build the solution directly to the release bin directory
Write-Host "Building solution..."
dotnet build "$rootPath\PSProxmox\PSProxmox.Main.csproj" -c Release -o $releaseBinDir

# Copy the module files to the release directory
Copy-Item -Path "$rootPath\Module\PSProxmox.psd1" -Destination $releaseVersionDir -Force
Copy-Item -Path "$rootPath\LICENSE" -Destination $releaseVersionDir -Force
Copy-Item -Path "$rootPath\README.md" -Destination $releaseVersionDir -Force
Copy-Item -Path "$scriptRoot\Install-PSProxmox.ps1" -Destination $releaseVersionDir -Force

# Copy the fully prepared module back to the Module directory
Write-Host "Updating Module directory with built files..."
Copy-Item -Path "$releaseBinDir\PSProxmox.dll" -Destination "$rootPath\Module\bin" -Force

# Create a ZIP file of the release
$zipPath = "$releaseBaseDir\PSProxmox-$version.zip"
Compress-Archive -Path "$releaseVersionDir\*" -DestinationPath $zipPath -Force
Write-Host "Created release package: $zipPath"

Write-Host "Build completed successfully!"
