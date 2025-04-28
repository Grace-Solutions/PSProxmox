# Cluster-CreateBackup.ps1
# This script demonstrates how to create a cluster backup using the PSProxmox module.

# Import the PSProxmox module
Import-Module PSProxmox

# Connect to the Proxmox VE server
$credential = Get-Credential -Message "Enter your Proxmox VE credentials"
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential $credential -Realm "pam"

# Method 1: Create a cluster backup with default settings
$backup = New-ProxmoxClusterBackup -Connection $connection

# Display the backup information
$backup

# Method 2: Create a cluster backup with compression
$backup = New-ProxmoxClusterBackup -Connection $connection -Compress

# Display the backup information
$backup

# Method 3: Create a cluster backup and wait for it to complete
$backup = New-ProxmoxClusterBackup -Connection $connection -Compress -Wait -Timeout 600

# Display the backup information
$backup

# Method 4: Get all cluster backups
$backups = Get-ProxmoxClusterBackup -Connection $connection

# Display the backups
$backups

# Method 5: Get a specific cluster backup
$backupID = $backup.BackupID
$specificBackup = Get-ProxmoxClusterBackup -Connection $connection -BackupID $backupID

# Display the specific backup
$specificBackup

# Disconnect from the server when done
Disconnect-ProxmoxServer -Connection $connection
