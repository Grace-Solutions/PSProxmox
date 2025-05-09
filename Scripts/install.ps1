# Install the PSProxmox module to the current user's module directory

# Get the latest release
$releaseDir = Get-ChildItem -Path "$PSScriptRoot\..\Release" -Directory | Sort-Object Name -Descending | Select-Object -First 1

if (-not $releaseDir) {
    Write-Error "No release found. Please build the module first."
    exit 1
}

$modulePath = "$env:USERPROFILE\Documents\WindowsPowerShell\Modules\PSProxmox"

# Create the module directory if it doesn't exist
if (-not (Test-Path $modulePath)) {
    New-Item -Path $modulePath -ItemType Directory -Force | Out-Null
}

# Copy the module files
Copy-Item -Path "$PSScriptRoot\..\Module\*" -Destination $modulePath -Recurse -Force

Write-Host "PSProxmox module installed to $modulePath"

# Import the module
Import-Module PSProxmox -Force

# List the cloud image cmdlets
Get-Command -Module PSProxmox | Where-Object { $_.Name -like "*Cloud*" }
