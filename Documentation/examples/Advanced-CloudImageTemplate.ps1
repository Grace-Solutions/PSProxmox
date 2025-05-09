# Advanced-CloudImageTemplate.ps1
# This script demonstrates advanced usage of cloud image template functionality.

# Parameters
param(
    [Parameter(Mandatory = $false)]
    [string]$ProxmoxServer = "proxmox.example.com",
    
    [Parameter(Mandatory = $false)]
    [string]$Node = "pve1",
    
    [Parameter(Mandatory = $false)]
    [string]$Storage = "local-lvm",
    
    [Parameter(Mandatory = $false)]
    [string]$Distribution = "ubuntu",
    
    [Parameter(Mandatory = $false)]
    [string]$Release = "22.04",
    
    [Parameter(Mandatory = $false)]
    [string]$Variant = "server",
    
    [Parameter(Mandatory = $false)]
    [int]$MemoryMB = 2048,
    
    [Parameter(Mandatory = $false)]
    [int]$Cores = 2,
    
    [Parameter(Mandatory = $false)]
    [int]$DiskSizeGB = 20,
    
    [Parameter(Mandatory = $false)]
    [string]$TemplateNamePrefix = "cloud",
    
    [Parameter(Mandatory = $false)]
    [string]$VMNamePrefix = "vm",
    
    [Parameter(Mandatory = $false)]
    [int]$VMCount = 3,
    
    [Parameter(Mandatory = $false)]
    [string]$SSHKeyPath = "~/.ssh/id_rsa.pub",
    
    [Parameter(Mandatory = $false)]
    [string]$Username = "admin",
    
    [Parameter(Mandatory = $false)]
    [string]$Password = "SecurePassword123!",
    
    [Parameter(Mandatory = $false)]
    [string]$IPConfigTemplate = "ip=192.168.1.{0}/24,gw=192.168.1.1",
    
    [Parameter(Mandatory = $false)]
    [string]$DNS = "8.8.8.8,8.8.4.4",
    
    [Parameter(Mandatory = $false)]
    [switch]$CustomizeImage,
    
    [Parameter(Mandatory = $false)]
    [switch]$StartVMs
)

# Function to log messages with timestamps
function Write-Log {
    param([string]$Message)
    Write-Host "$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss') - $Message"
}

try {
    # Connect to the Proxmox server
    Write-Log "Connecting to Proxmox server $ProxmoxServer..."
    $securePassword = ConvertTo-SecureString $Password -AsPlainText -Force
    $credential = New-Object System.Management.Automation.PSCredential($Username, $securePassword)
    Connect-ProxmoxServer -Server $ProxmoxServer -Credential $credential
    Write-Log "Connected to Proxmox server $ProxmoxServer"
    
    # Get available cloud images
    Write-Log "Getting available $Distribution $Release $Variant cloud images..."
    $images = Get-ProxmoxCloudImage -Distribution $Distribution -Release $Release
    if ($images.Count -eq 0) {
        throw "No cloud images found for $Distribution $Release"
    }
    Write-Log "Found $($images.Count) cloud images"
    
    # Download the cloud image
    Write-Log "Downloading $Distribution $Release $Variant cloud image..."
    $imagePath = Save-ProxmoxCloudImage -Distribution $Distribution -Release $Release -Variant $Variant
    Write-Log "Downloaded cloud image to: $imagePath"
    
    # Customize the cloud image if requested
    if ($CustomizeImage) {
        Write-Log "Customizing cloud image (resizing to $DiskSizeGB GB)..."
        $imagePath = Invoke-ProxmoxCloudImageCustomization -ImagePath $imagePath -Resize $DiskSizeGB
        Write-Log "Customized cloud image: $imagePath"
    }
    
    # Create a template name with timestamp
    $timestamp = Get-Date -Format "yyyyMMddHHmmss"
    $templateName = "$TemplateNamePrefix-$Distribution-$Release-$timestamp"
    
    # Create a template from the cloud image
    Write-Log "Creating template $templateName from cloud image..."
    $template = New-ProxmoxCloudImageTemplate -Node $Node -Name $templateName -ImagePath $imagePath -Storage $Storage -Memory $MemoryMB -Cores $Cores -DiskSize $DiskSizeGB
    Write-Log "Created template: $($template.Name) (VMID: $($template.VMID))"
    
    # Read SSH key if path is provided
    $sshKey = $null
    if (Test-Path $SSHKeyPath) {
        $sshKey = Get-Content -Path $SSHKeyPath -Raw
        Write-Log "Read SSH key from $SSHKeyPath"
    }
    
    # Create VMs from the template
    Write-Log "Creating $VMCount VMs from template $templateName..."
    $vms = @()
    for ($i = 1; $i -le $VMCount; $i++) {
        $vmName = "$VMNamePrefix-$i"
        Write-Log "Creating VM $vmName..."
        $vm = New-ProxmoxVMFromTemplate -Node $Node -TemplateName $templateName -Name $vmName
        Write-Log "Created VM: $($vm.Name) (VMID: $($vm.VMID))"
        
        # Set Cloud-Init configuration for the VM
        $ipConfig = $IPConfigTemplate -f (100 + $i)
        Write-Log "Setting Cloud-Init configuration for VM $($vm.VMID) with IP $ipConfig..."
        $securePassword = ConvertTo-SecureString $Password -AsPlainText -Force
        Set-ProxmoxVMCloudInit -Node $Node -VMID $vm.VMID -Username $Username -Password $securePassword -SSHKey $sshKey -IPConfig $ipConfig -DNS $DNS
        Write-Log "Set Cloud-Init configuration for VM $($vm.VMID)"
        
        # Start the VM if requested
        if ($StartVMs) {
            Write-Log "Starting VM $($vm.VMID)..."
            Start-ProxmoxVM -Node $Node -VMID $vm.VMID
            Write-Log "Started VM $($vm.VMID)"
        }
        
        $vms += $vm
    }
    
    # Display summary
    Write-Log "Summary:"
    Write-Log "Template: $($template.Name) (VMID: $($template.VMID))"
    Write-Log "VMs created:"
    $vms | ForEach-Object {
        Write-Log "  - $($_.Name) (VMID: $($_.VMID))"
    }
}
catch {
    Write-Log "Error: $_"
}
finally {
    # Disconnect from the Proxmox server
    if (Test-ProxmoxConnection) {
        Write-Log "Disconnecting from Proxmox server..."
        Disconnect-ProxmoxServer
        Write-Log "Disconnected from Proxmox server"
    }
}
