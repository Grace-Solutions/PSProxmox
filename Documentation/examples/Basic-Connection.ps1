# Basic-Connection.ps1
# This script demonstrates how to connect to a Proxmox VE server using the PSProxmox module.

# Import the PSProxmox module
Import-Module PSProxmox

# Method 1: Connect using username and password
$securePassword = ConvertTo-SecureString "password" -AsPlainText -Force
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Username "root" -Password $securePassword -Realm "pam"

# Method 2: Connect using a credential object
$credential = Get-Credential -Message "Enter your Proxmox VE credentials"
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Credential $credential -Realm "pam"

# Method 3: Connect using an API token
$secureSecret = ConvertTo-SecureString "secret" -AsPlainText -Force
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -ApiToken "root@pam!token" -ApiTokenSecret $secureSecret

# Method 4: Connect with SSL certificate validation disabled (not recommended for production)
$securePassword = ConvertTo-SecureString "password" -AsPlainText -Force
$connection = Connect-ProxmoxServer -Server "proxmox.example.com" -Username "root" -Password $securePassword -Realm "pam" -SkipCertificateValidation $true

# Display the connection information
$connection

# Disconnect from the server when done
Disconnect-ProxmoxServer -Connection $connection
