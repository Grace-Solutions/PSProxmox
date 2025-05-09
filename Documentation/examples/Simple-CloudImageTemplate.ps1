# Simple-CloudImageTemplate.ps1
# This script demonstrates the basic usage of cloud image template functionality.

# Connect to the Proxmox server
Connect-ProxmoxServer -Server "proxmox.example.com" -Credential (Get-Credential)

# List available Ubuntu cloud images
Get-ProxmoxCloudImage -Distribution "ubuntu" -Release "22.04"

# Download an Ubuntu 22.04 server cloud image
$imagePath = Save-ProxmoxCloudImage -Distribution "ubuntu" -Release "22.04" -Variant "server"
Write-Host "Downloaded cloud image to: $imagePath"

# Create a template from the cloud image
$template = New-ProxmoxCloudImageTemplate -Node "pve1" -Name "ubuntu-22.04" -ImagePath $imagePath -Storage "local-lvm"
Write-Host "Created template: $($template.Name) (VMID: $($template.VMID))"

# Create a VM from the template
$vm = New-ProxmoxVMFromTemplate -Node "pve1" -TemplateName "ubuntu-22.04" -Name "web01"
Write-Host "Created VM: $($vm.Name) (VMID: $($vm.VMID))"

# Set Cloud-Init configuration for the VM
$password = ConvertTo-SecureString "SecurePassword123!" -AsPlainText -Force
Set-ProxmoxVMCloudInit -Node "pve1" -VMID $vm.VMID -Username "admin" -Password $password -IPConfig "dhcp"
Write-Host "Set Cloud-Init configuration for VM $($vm.VMID)"

# Start the VM
Start-ProxmoxVM -Node "pve1" -VMID $vm.VMID
Write-Host "Started VM $($vm.VMID)"

# Disconnect from the Proxmox server
Disconnect-ProxmoxServer
Write-Host "Disconnected from Proxmox server"
