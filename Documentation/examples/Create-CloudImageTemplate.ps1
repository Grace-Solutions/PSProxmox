# Create-CloudImageTemplate.ps1
# This script demonstrates how to create a template from a cloud image and deploy VMs from it.

# Connect to the Proxmox server
$securePassword = ConvertTo-SecureString "password" -AsPlainText -Force
Connect-ProxmoxServer -Server "proxmox.example.com" -Username "root" -Password $securePassword -Realm "pam"

# Get available Ubuntu cloud images
$ubuntuImages = Get-ProxmoxCloudImage -Distribution "ubuntu" -Release "22.04"
Write-Host "Available Ubuntu 22.04 cloud images:"
$ubuntuImages | Format-Table Distribution, Release, Variant, Format

# Download the Ubuntu 22.04 server cloud image
$imagePath = Save-ProxmoxCloudImage -Distribution "ubuntu" -Release "22.04" -Variant "server"
Write-Host "Downloaded cloud image to: $imagePath"

# Customize the cloud image (resize to 20GB)
$customizedImagePath = Invoke-ProxmoxCloudImageCustomization -ImagePath $imagePath -Resize 20
Write-Host "Customized cloud image: $customizedImagePath"

# Create a template from the cloud image
$template = New-ProxmoxCloudImageTemplate -Node "pve1" -Name "ubuntu-22.04-template" -ImagePath $customizedImagePath -Storage "local-lvm" -Memory 2048 -Cores 2 -DiskSize 20
Write-Host "Created template: $($template.Name) (VMID: $($template.VMID))"

# Create a VM from the template
$vm = New-ProxmoxVMFromTemplate -Node "pve1" -TemplateName "ubuntu-22.04-template" -Name "web01"
Write-Host "Created VM: $($vm.Name) (VMID: $($vm.VMID))"

# Set Cloud-Init configuration for the VM
$sshKey = Get-Content -Path "~/.ssh/id_rsa.pub"
$password = ConvertTo-SecureString "SecurePassword123!" -AsPlainText -Force
Set-ProxmoxVMCloudInit -Node "pve1" -VMID $vm.VMID -Username "admin" -Password $password -SSHKey $sshKey -IPConfig "dhcp" -DNS "8.8.8.8,8.8.4.4"
Write-Host "Set Cloud-Init configuration for VM $($vm.VMID)"

# Start the VM
Start-ProxmoxVM -Node "pve1" -VMID $vm.VMID
Write-Host "Started VM $($vm.VMID)"

# Create multiple VMs from the template
$vms = New-ProxmoxVMFromTemplate -Node "pve1" -TemplateName "ubuntu-22.04-template" -Prefix "web" -Count 3 -Start
Write-Host "Created multiple VMs:"
$vms | Format-Table VMID, Name, Node, Status

# Disconnect from the Proxmox server
Disconnect-ProxmoxServer
Write-Host "Disconnected from Proxmox server"
