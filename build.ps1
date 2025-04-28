# build.ps1
# This script builds the PSProxmox module and creates a release package

# Set the version based on the current date and time
$version = Get-Date -Format "yyyy.MM.dd.HHmm"
Write-Host "Building PSProxmox version $version"

# Update the module version in the PSD1 file
$psd1Path = ".\Module\PSProxmox.psd1"
$psd1Content = Get-Content -Path $psd1Path -Raw
$psd1Content = $psd1Content -replace "ModuleVersion = '.*'", "ModuleVersion = '$version'"
Set-Content -Path $psd1Path -Value $psd1Content

# Build the solution
Write-Host "Building solution..."
dotnet build .\PSProxmox\PSProxmox.Main.csproj -c Release

# Create the release directory
$releaseDir = ".\Release\$version"
if (-not (Test-Path -Path $releaseDir)) {
    New-Item -Path $releaseDir -ItemType Directory -Force | Out-Null
    Write-Host "Created release directory: $releaseDir"
}

# Copy the module files to the release directory
Copy-Item -Path .\Module\PSProxmox.psd1 -Destination $releaseDir -Force
Copy-Item -Path .\Module\bin\PSProxmox.dll -Destination $releaseDir -Force
Copy-Item -Path .\Module\LICENSE -Destination $releaseDir -Force
Copy-Item -Path .\Module\README.md -Destination $releaseDir -Force
Copy-Item -Path .\Module\Install-PSProxmox.ps1 -Destination $releaseDir -Force

# Create a bin directory in the release directory
$releaseBinDir = "$releaseDir\bin"
if (-not (Test-Path -Path $releaseBinDir)) {
    New-Item -Path $releaseBinDir -ItemType Directory -Force | Out-Null
    Write-Host "Created bin directory: $releaseBinDir"
}

# Copy the DLL to the bin directory
Copy-Item -Path .\Module\bin\PSProxmox.dll -Destination $releaseBinDir -Force

# Create a ZIP file of the release
$zipPath = ".\Release\PSProxmox-$version.zip"
Compress-Archive -Path $releaseDir\* -DestinationPath $zipPath -Force
Write-Host "Created release package: $zipPath"

Write-Host "Build completed successfully!"
