using System;
using System.Management.Automation;
using PSProxmox.Client;
using PSProxmox.Models;
using PSProxmox.Session;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Removes a virtual machine from Proxmox VE.</para>
    /// <para type="description">The Remove-ProxmoxVM cmdlet removes a virtual machine from Proxmox VE.</para>
    /// <example>
    ///   <para>Remove a virtual machine</para>
    ///   <code>Remove-ProxmoxVM -Connection $connection -Node "pve1" -VMID 100</code>
    /// </example>
    /// <example>
    ///   <para>Remove a virtual machine using pipeline input</para>
    ///   <code>Get-ProxmoxVM -Connection $connection -VMID 100 | Remove-ProxmoxVM -Connection $connection</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "ProxmoxVM", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveProxmoxVMCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The node where the VM is located.</para>
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public string Node { get; set; }

        /// <summary>
        /// <para type="description">The ID of the VM to remove.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public int VMID { get; set; }

        /// <summary>
        /// <para type="description">Whether to force removal of the VM.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        /// <summary>
        /// <para type="description">Whether to purge the VM's files.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Purge { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);

                // If node is not specified, find the VM on any node
                if (string.IsNullOrEmpty(Node))
                {
                    string nodesResponse = client.Get("nodes");
                    var nodesData = Newtonsoft.Json.Linq.JObject.Parse(nodesResponse)["data"] as Newtonsoft.Json.Linq.JArray;

                    foreach (var nodeObj in nodesData)
                    {
                        string nodeName = nodeObj["node"].ToString();
                        try
                        {
                            // Check if the VM exists on this node
                            client.Get($"nodes/{nodeName}/qemu/{VMID}/status/current");
                            Node = nodeName;
                            break;
                        }
                        catch
                        {
                            // VM not found on this node, continue to the next one
                        }
                    }

                    if (string.IsNullOrEmpty(Node))
                    {
                        WriteError(new ErrorRecord(
                            new Exception($"VM with ID {VMID} not found on any node"),
                            "VMNotFound",
                            ErrorCategory.ObjectNotFound,
                            VMID));
                        return;
                    }
                }

                // Check if the VM is running
                string vmResponse = client.Get($"nodes/{Node}/qemu/{VMID}/status/current");
                var vmData = Newtonsoft.Json.Linq.JObject.Parse(vmResponse)["data"] as Newtonsoft.Json.Linq.JObject;
                string status = vmData["status"].ToString();

                if (status == "running" && !Force.IsPresent)
                {
                    WriteError(new ErrorRecord(
                        new Exception($"VM with ID {VMID} is running. Use -Force to stop and remove it."),
                        "VMRunning",
                        ErrorCategory.InvalidOperation,
                        VMID));
                    return;
                }

                // Confirm removal
                if (!ShouldProcess($"VM {VMID} on node {Node}", "Remove"))
                {
                    return;
                }

                // Stop the VM if it's running
                if (status == "running")
                {
                    WriteVerbose($"Stopping VM {VMID} on node {Node}");
                    client.Post($"nodes/{Node}/qemu/{VMID}/status/stop", null);

                    // Wait for the VM to stop
                    int attempts = 0;
                    while (attempts < 10)
                    {
                        vmResponse = client.Get($"nodes/{Node}/qemu/{VMID}/status/current");
                        vmData = Newtonsoft.Json.Linq.JObject.Parse(vmResponse)["data"] as Newtonsoft.Json.Linq.JObject;
                        status = vmData["status"].ToString();

                        if (status != "running")
                        {
                            break;
                        }

                        System.Threading.Thread.Sleep(1000);
                        attempts++;
                    }

                    if (status == "running")
                    {
                        WriteWarning($"VM {VMID} did not stop gracefully. Forcing removal.");
                    }
                }

                // Remove the VM
                WriteVerbose($"Removing VM {VMID} from node {Node}");
                string deleteUrl = $"nodes/{Node}/qemu/{VMID}";
                if (Purge.IsPresent)
                {
                    deleteUrl += "?purge=1";
                }
                client.Delete(deleteUrl);

                WriteVerbose($"VM {VMID} removed from node {Node}");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "RemoveProxmoxVMError", ErrorCategory.OperationStopped, VMID));
            }
        }
    }
}
