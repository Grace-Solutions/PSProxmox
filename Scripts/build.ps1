# build.ps1
# This script builds the PSProxmox module and creates a release package

param(
    [switch]$Publish,
    [string]$NuGetApiKey = $env:NUGET_API_KEY
)

# Set script root path for relative paths
$scriptRoot = $PSScriptRoot
$rootPath = Split-Path -Parent $scriptRoot

# Get the version from the module manifest
$psd1Path = "$rootPath\Module\PSProxmox.psd1"
$psd1Content = Get-Content -Path $psd1Path -Raw
if ($psd1Content -match "ModuleVersion\s*=\s*'([^']+)'") {
    $version = $matches[1]
} else {
    # Fallback to current date and time if version not found in manifest
    $version = Get-Date -Format "yyyy.MM.dd.HHmm"
}
Write-Host "Building PSProxmox version $version"

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

# Copy the module files to the release directory (source from PSProxmox project)
Copy-Item -Path "$rootPath\PSProxmox\PSProxmox.psd1" -Destination $releaseVersionDir -Force
Copy-Item -Path "$rootPath\LICENSE" -Destination $releaseVersionDir -Force
Copy-Item -Path "$rootPath\README.md" -Destination $releaseVersionDir -Force
Copy-Item -Path "$scriptRoot\Install-PSProxmox.ps1" -Destination $releaseVersionDir -Force

# Make sure the bin directory exists in the release directory
if (-not (Test-Path -Path "$releaseVersionDir\bin")) {
    New-Item -Path "$releaseVersionDir\bin" -ItemType Directory -Force | Out-Null
    Write-Host "Created bin directory in release: $releaseVersionDir\bin"
}

# Copy the fully prepared module back to the Module directory
Write-Host "Updating Module directory with built files..."
Copy-Item -Path "$releaseBinDir\PSProxmox.dll" -Destination "$rootPath\Module\bin" -Force

# Copy dependencies to the Module directory (all dependencies now in bin)
Write-Host "Copying dependencies to Module directory..."
Copy-Item -Path "$releaseBinDir\Newtonsoft.Json.dll" -Destination "$rootPath\Module\bin" -Force

# Copy the source manifest to Module directory
Write-Host "Copying source manifest to Module directory..."
Copy-Item -Path "$rootPath\PSProxmox\PSProxmox.psd1" -Destination "$rootPath\Module" -Force

# Create a ZIP file of the release
$zipPath = "$releaseBaseDir\PSProxmox-$version.zip"
Compress-Archive -Path "$releaseVersionDir\*" -DestinationPath $zipPath -Force
Write-Host "Created release package: $zipPath"

Write-Host "Build completed successfully!"

# Publish to PowerShell Gallery if requested
if ($Publish) {
    Write-Host "Publishing to PowerShell Gallery..."

    if (-not $NuGetApiKey) {
        Write-Error "NuGetApiKey is required for publishing. Set the NUGET_API_KEY environment variable or pass -NuGetApiKey parameter."
        exit 1
    }

    # Use the existing Module\PSProxmox structure that we've already created
    $publishModuleDir = "$rootPath\Module\PSProxmox"

    # Ensure the version directory has the latest source files
    $publishVersionDir = "$publishModuleDir\$version"

    # Copy the source psd1 to version directory (maintaining relative paths)
    Copy-Item -Path "$rootPath\PSProxmox\PSProxmox.psd1" -Destination $publishVersionDir -Force

    # Copy the source psd1 to PSProxmox level for Publish-Module to find
    Copy-Item -Path "$rootPath\PSProxmox\PSProxmox.psd1" -Destination $publishModuleDir -Force

    try {
        # Enable TLS 1.2 for PowerShell Gallery
        [Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

        # Publish from the PSProxmox level (not the version level)
        Publish-Module -Path $publishModuleDir -NuGetApiKey $NuGetApiKey -LicenseUri 'https://github.com/Grace-Solutions/PSProxmox/blob/main/LICENSE' -Tag 'Proxmox','VirtualMachine','Cluster','Management' -ReleaseNotes "Updated to .NET Standard 2.0 for dual PowerShell 5.1/7+ support. Added missing SMBIOS cmdlets. Complete compatibility with both Windows PowerShell and PowerShell Core. Version consistency across all binaries." -Verbose
        Write-Host "Successfully published PSProxmox version $version to PowerShell Gallery!" -ForegroundColor Green
    }
    catch {
        Write-Error "Failed to publish module: $($_.Exception.Message)"
        Write-Host "Module structure preserved at: $publishModuleDir"
        exit 1
    }
}
