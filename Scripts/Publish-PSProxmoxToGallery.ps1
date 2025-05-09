# Publish-PSProxmoxToGallery.ps1
# This script publishes the PSProxmox module to the PowerShell Gallery.

param(
    [Parameter(Mandatory = $true)]
    [string]$ApiKey,

    [Parameter(Mandatory = $false)]
    [string]$Version = (Get-Date -Format "yyyy.MM.dd.HHmm")
)

# Ensure the module is built
Write-Host "Building the module with version $Version..."
$env:MODULE_VERSION = $Version
& "$PSScriptRoot\build-release-only.ps1"

# Get the release directory
$releaseDir = Join-Path -Path (Split-Path -Parent $PSScriptRoot) -ChildPath "Release\$Version"

if (-not (Test-Path -Path $releaseDir)) {
    Write-Error "Release directory not found: $releaseDir"
    exit 1
}

# Publish the module to PowerShell Gallery
Write-Host "Publishing module to PowerShell Gallery..."
try {
    Publish-Module -Path $releaseDir -NuGetApiKey $ApiKey -Verbose
    Write-Host "Module published successfully!" -ForegroundColor Green
}
catch {
    Write-Error "Failed to publish module: $_"
    exit 1
}

# Open the PowerShell Gallery page
$moduleName = "PSProxmox"
Start-Process "https://www.powershellgallery.com/packages/$moduleName"

Write-Host "Done!"
