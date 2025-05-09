# Working with Cloud Image Templates

This guide explains how to work with cloud image templates in PSProxmox. Cloud images are pre-built, minimal operating system images designed for cloud environments. They typically come with Cloud-Init pre-installed, which allows for easy customization at boot time.

## Prerequisites

- PSProxmox module installed
- Connection to a Proxmox VE server
- QEMU tools installed on the system (for image customization)

## Finding Available Cloud Images

You can use the `Get-ProxmoxCloudImage` cmdlet to find available cloud images:

```powershell
# Get all available cloud images
Get-ProxmoxCloudImage

# Get Ubuntu cloud images
Get-ProxmoxCloudImage -Distribution "ubuntu"

# Get Ubuntu 22.04 cloud images
Get-ProxmoxCloudImage -Distribution "ubuntu" -Release "22.04"
```

## Downloading Cloud Images

You can use the `Save-ProxmoxCloudImage` cmdlet to download cloud images:

```powershell
# Download an Ubuntu 22.04 server cloud image
$imagePath = Save-ProxmoxCloudImage -Distribution "ubuntu" -Release "22.04" -Variant "server"

# Download a Debian 11 generic cloud image to a specific location
$imagePath = Save-ProxmoxCloudImage -Distribution "debian" -Release "11" -Variant "generic" -OutputPath "C:\Images\debian-11.qcow2"
```

## Customizing Cloud Images

You can use the `Invoke-ProxmoxCloudImageCustomization` cmdlet to customize cloud images:

```powershell
# Resize a cloud image to 20GB
$customizedImagePath = Invoke-ProxmoxCloudImageCustomization -ImagePath $imagePath -Resize 20

# Convert a cloud image to qcow2 format
$customizedImagePath = Invoke-ProxmoxCloudImageCustomization -ImagePath $imagePath -ConvertTo "qcow2"

# Resize and convert a cloud image
$customizedImagePath = Invoke-ProxmoxCloudImageCustomization -ImagePath $imagePath -Resize 20 -ConvertTo "qcow2" -OutputPath "C:\Images\ubuntu-22.04-custom.qcow2"
```

## Creating Templates from Cloud Images

You can use the `New-ProxmoxCloudImageTemplate` cmdlet to create templates from cloud images:

```powershell
# Create a template from an Ubuntu 22.04 cloud image
$template = New-ProxmoxCloudImageTemplate -Node "pve1" -Name "ubuntu-22.04" -Distribution "ubuntu" -Release "22.04" -Storage "local-lvm" -Memory 2048 -Cores 2 -DiskSize 20

# Create a template from a local cloud image file
$template = New-ProxmoxCloudImageTemplate -Node "pve1" -Name "ubuntu-22.04" -ImagePath "C:\Images\ubuntu-22.04-server-cloudimg-amd64.img" -Storage "local-lvm" -Memory 2048 -Cores 2 -DiskSize 20
```

## Setting Cloud-Init Configuration

You can use the `Set-ProxmoxVMCloudInit` cmdlet to set Cloud-Init configuration for a VM:

```powershell
# Set Cloud-Init configuration for a VM
$password = ConvertTo-SecureString "password" -AsPlainText -Force
$sshKey = Get-Content -Path "~/.ssh/id_rsa.pub"
Set-ProxmoxVMCloudInit -Node "pve1" -VMID 100 -Username "admin" -Password $password -SSHKey $sshKey -IPConfig "dhcp" -DNS "8.8.8.8,8.8.4.4"

# Set Cloud-Init configuration with static IP
Set-ProxmoxVMCloudInit -Node "pve1" -VMID 100 -Username "admin" -Password $password -IPConfig "ip=192.168.1.100/24,gw=192.168.1.1"
```

## Creating VMs from Templates

You can use the `New-ProxmoxVMFromTemplate` cmdlet to create VMs from templates:

```powershell
# Create a VM from a template
$vm = New-ProxmoxVMFromTemplate -Node "pve1" -TemplateName "ubuntu-22.04" -Name "web01"

# Create multiple VMs from a template
$vms = New-ProxmoxVMFromTemplate -Node "pve1" -TemplateName "ubuntu-22.04" -Prefix "web" -Count 3 -Start
```

## Complete Example

Here's a complete example that demonstrates the entire workflow:

```powershell
# Connect to the Proxmox server
$securePassword = ConvertTo-SecureString "password" -AsPlainText -Force
Connect-ProxmoxServer -Server "proxmox.example.com" -Username "root" -Password $securePassword -Realm "pam"

# Download the Ubuntu 22.04 server cloud image
$imagePath = Save-ProxmoxCloudImage -Distribution "ubuntu" -Release "22.04" -Variant "server"

# Customize the cloud image (resize to 20GB)
$customizedImagePath = Invoke-ProxmoxCloudImageCustomization -ImagePath $imagePath -Resize 20

# Create a template from the cloud image
$template = New-ProxmoxCloudImageTemplate -Node "pve1" -Name "ubuntu-22.04-template" -ImagePath $customizedImagePath -Storage "local-lvm" -Memory 2048 -Cores 2 -DiskSize 20

# Create a VM from the template
$vm = New-ProxmoxVMFromTemplate -Node "pve1" -TemplateName "ubuntu-22.04-template" -Name "web01"

# Set Cloud-Init configuration for the VM
$sshKey = Get-Content -Path "~/.ssh/id_rsa.pub"
$password = ConvertTo-SecureString "SecurePassword123!" -AsPlainText -Force
Set-ProxmoxVMCloudInit -Node "pve1" -VMID $vm.VMID -Username "admin" -Password $password -SSHKey $sshKey -IPConfig "dhcp" -DNS "8.8.8.8,8.8.4.4"

# Start the VM
Start-ProxmoxVM -Node "pve1" -VMID $vm.VMID

# Disconnect from the Proxmox server
Disconnect-ProxmoxServer
```

## Best Practices

- Use cloud images from official sources to ensure security and reliability.
- Customize cloud images before creating templates to save time when deploying multiple VMs.
- Use Cloud-Init to configure VMs at boot time instead of manually configuring each VM.
- Use templates to ensure consistency across multiple VMs.
- Use descriptive names for templates and VMs to make them easier to identify.

## Troubleshooting

- If you encounter issues with cloud image downloads, check your internet connection and try again.
- If you encounter issues with image customization, ensure that QEMU tools are installed on your system.
- If you encounter issues with template creation, check the Proxmox VE server logs for more information.
- If you encounter issues with Cloud-Init configuration, ensure that the VM has a Cloud-Init drive attached.
