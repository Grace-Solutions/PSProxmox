# LXC Container Management Examples

# Connect to the Proxmox server
$securePassword = ConvertTo-SecureString "password" -AsPlainText -Force
Connect-ProxmoxServer -Server "proxmox.example.com" -Username "root" -Password $securePassword -Realm "pam"

# Get all LXC containers
$containers = Get-ProxmoxContainer
$containers | Format-Table -Property CTID, Name, Status, Node

# Get a specific LXC container
$container = Get-ProxmoxContainer -CTID 100
$container

# Get all LXC containers on a specific node
$nodeContainers = Get-ProxmoxContainer -Node "pve1"
$nodeContainers | Format-Table -Property CTID, Name, Status

# Get LXC containers by name pattern (wildcard)
$webContainers = Get-ProxmoxContainer -Name "web*"
$webContainers | Format-Table -Property CTID, Name, Status, Node

# Get LXC containers by name pattern (regex)
$dbContainers = Get-ProxmoxContainer -Name "^db\d+$"
$dbContainers | Format-Table -Property CTID, Name, Status, Node

# Create a new LXC container using parameters
$container = New-ProxmoxContainer -Node "pve1" -Name "web-container" -OSTemplate "local:vztmpl/ubuntu-20.04-standard_20.04-1_amd64.tar.gz" -Storage "local-lvm" -Memory 512 -Swap 512 -Cores 1 -DiskSize 8 -Unprivileged -StartOnBoot -Start
$container

# Create a new LXC container using a builder
$builder = New-ProxmoxContainerBuilder -Name "db-container"
$builder.WithOSTemplate("local:vztmpl/ubuntu-20.04-standard_20.04-1_amd64.tar.gz")
        .WithStorage("local-lvm")
        .WithMemory(1024)
        .WithSwap(1024)
        .WithCores(2)
        .WithDiskSize(16)
        .WithUnprivileged($true)
        .WithStartOnBoot($true)
        .WithStart($true)
        .WithDescription("Database container")

$container = New-ProxmoxContainer -Node "pve1" -Builder $builder
$container

# Start an LXC container
Start-ProxmoxContainer -Node "pve1" -CTID 100 -Wait
Get-ProxmoxContainer -CTID 100

# Stop an LXC container
Stop-ProxmoxContainer -Node "pve1" -CTID 100 -Wait
Get-ProxmoxContainer -CTID 100

# Restart an LXC container
Restart-ProxmoxContainer -Node "pve1" -CTID 100 -Wait
Get-ProxmoxContainer -CTID 100

# Remove an LXC container
Remove-ProxmoxContainer -Node "pve1" -CTID 100 -Confirm:$false

# Get all TurnKey Linux templates
$templates = Get-ProxmoxTurnKeyTemplate
$templates | Format-Table -Property Name, Title, OS, Version, HumanSize

# Get TurnKey Linux templates by name pattern (wildcard)
$wordpressTemplates = Get-ProxmoxTurnKeyTemplate -Name "wordpress*"
$wordpressTemplates | Format-Table -Property Name, Title, OS, Version, HumanSize

# Download a TurnKey Linux template
$templatePath = Save-ProxmoxTurnKeyTemplate -Node "pve1" -Name "wordpress" -Storage "local"
$templatePath

# Create a new LXC container from a TurnKey Linux template
$container = New-ProxmoxContainerFromTurnKey -Node "pve1" -Name "wordpress" -Template "wordpress" -Storage "local-lvm" -Memory 512 -Cores 1 -DiskSize 8 -Start
$container

# Disconnect from the Proxmox server
Disconnect-ProxmoxServer
