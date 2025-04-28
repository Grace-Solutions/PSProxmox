# Cluster Backup and Restore

This guide explains how to backup and restore Proxmox VE clusters using the PSProxmox module.

## Prerequisites

- PSProxmox module installed
- Connection to a Proxmox VE server
- Administrative access to the Proxmox VE cluster

## Cluster Backups

Cluster backups in Proxmox VE contain the cluster configuration, including:
- Cluster configuration
- User and role configuration
- Storage configuration
- Network configuration
- Firewall configuration
- HA configuration

They do not include VM data or container data.

## Creating Cluster Backups

### Basic Backup

To create a basic cluster backup:

```powershell
# Connect to the Proxmox VE server
$credential = Get-Credential
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential $credential -Realm "pam"

# Create a cluster backup
$backup = New-ProxmoxClusterBackup -Connection $connection
```

### Backup with Compression

To create a compressed cluster backup:

```powershell
$backup = New-ProxmoxClusterBackup -Connection $connection -Compress
```

### Backup and Wait for Completion

To create a backup and wait for it to complete:

```powershell
$backup = New-ProxmoxClusterBackup -Connection $connection -Compress -Wait -Timeout 600
```

This will wait up to 10 minutes (600 seconds) for the backup to complete.

## Listing Cluster Backups

To list all cluster backups:

```powershell
$backups = Get-ProxmoxClusterBackup -Connection $connection
```

To get a specific backup by ID:

```powershell
$backupID = "vzdump-cluster-2023_04_28-12_00_00.vma.lzo"
$backup = Get-ProxmoxClusterBackup -Connection $connection -BackupID $backupID
```

## Restoring Cluster Backups

### Basic Restore

To restore a cluster backup:

```powershell
$backupID = "vzdump-cluster-2023_04_28-12_00_00.vma.lzo"
Restore-ProxmoxClusterBackup -Connection $connection -BackupID $backupID -Force
```

The `-Force` parameter is required to confirm the restore operation.

### Restore and Wait for Completion

To restore a backup and wait for it to complete:

```powershell
Restore-ProxmoxClusterBackup -Connection $connection -BackupID $backupID -Force -Wait -Timeout 600
```

This will wait up to 10 minutes (600 seconds) for the restore to complete.

## Best Practices

### Backup Frequency

- Create regular backups of your cluster configuration
- Automate backup creation using scheduled tasks
- Keep multiple backups to ensure you can restore to different points in time

### Backup Verification

- Verify that backups are created successfully
- Test restoring backups in a non-production environment
- Document the backup and restore procedures

### Backup Storage

- Store backups in a secure location
- Consider storing backups off-site
- Implement backup rotation to manage storage space

## Example: Automated Backup Script

Here's an example script for automated cluster backups:

```powershell
# Automated-Cluster-Backup.ps1
# This script creates a cluster backup and logs the result

# Import the PSProxmox module
Import-Module PSProxmox

# Connect to the Proxmox VE server
$securePassword = ConvertTo-SecureString "password" -AsPlainText -Force
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Username "root" -Password $securePassword -Realm "pam"

# Create a timestamp for the log
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

try {
    # Create a cluster backup
    $backup = New-ProxmoxClusterBackup -Connection $connection -Compress -Wait -Timeout 600
    
    # Log the successful backup
    $logMessage = "$timestamp - Backup created successfully: $($backup.BackupID)"
    Add-Content -Path "C:\Logs\ProxmoxBackup.log" -Value $logMessage
}
catch {
    # Log the error
    $logMessage = "$timestamp - Backup failed: $($_.Exception.Message)"
    Add-Content -Path "C:\Logs\ProxmoxBackup.log" -Value $logMessage
}
finally {
    # Disconnect from the server
    Disconnect-ProxmoxServer -Connection $connection
}
```

## Example: Restore Script

Here's an example script for restoring a cluster backup:

```powershell
# Restore-Cluster-Backup.ps1
# This script restores a cluster backup

# Import the PSProxmox module
Import-Module PSProxmox

# Connect to the Proxmox VE server
$credential = Get-Credential -Message "Enter your Proxmox VE credentials"
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential $credential -Realm "pam"

# Get the latest backup
$latestBackup = Get-ProxmoxClusterBackup -Connection $connection | Sort-Object -Property Time -Descending | Select-Object -First 1

if ($latestBackup) {
    Write-Host "Latest backup: $($latestBackup.BackupID) from $(Get-Date -Date $latestBackup.Time -Format 'yyyy-MM-dd HH:mm:ss')"
    
    # Confirm the restore operation
    $confirm = Read-Host "Do you want to restore this backup? (y/n)"
    
    if ($confirm -eq "y") {
        # Restore the backup
        Restore-ProxmoxClusterBackup -Connection $connection -BackupID $latestBackup.BackupID -Force -Wait -Timeout 600
        Write-Host "Backup restored successfully"
    }
    else {
        Write-Host "Restore operation cancelled"
    }
}
else {
    Write-Host "No backups found"
}

# Disconnect from the server
Disconnect-ProxmoxServer -Connection $connection
```

## Troubleshooting

### Backup Creation Fails

If backup creation fails, check:
- The user has sufficient permissions
- There is enough disk space
- The storage is accessible
- The cluster is healthy

### Restore Operation Fails

If restore operation fails, check:
- The backup file exists and is accessible
- The backup file is not corrupted
- The user has sufficient permissions
- The cluster is in a state that allows restore operations

## Next Steps

Now that you know how to backup and restore Proxmox VE clusters, you can proceed to the [High Availability](High-Availability.md) guide to learn how to configure high availability for your cluster.
