using System;
using System.Management.Automation;
using PSProxmox.Client;
using PSProxmox.Models;
using PSProxmox.Session;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Restarts a virtual machine in Proxmox VE.</para>
    /// <para type="description">The Restart-ProxmoxVM cmdlet restarts a virtual machine in Proxmox VE.</para>
    /// <example>
    ///   <para>Restart a virtual machine</para>
    ///   <code>Restart-ProxmoxVM -Connection $connection -Node "pve1" -VMID 100</code>
    /// </example>
    /// <example>
    ///   <para>Restart a virtual machine using pipeline input</para>
    ///   <code>Get-ProxmoxVM -Connection $connection -VMID 100 | Restart-ProxmoxVM -Connection $connection</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsLifecycle.Restart, "ProxmoxVM", SupportsShouldProcess = true)]
    [OutputType(typeof(ProxmoxVM))]
    public class RestartProxmoxVMCmdlet : PSCmdlet
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
        /// <para type="description">The ID of the VM to restart.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public int VMID { get; set; }

        /// <summary>
        /// <para type="description">Whether to wait for the VM to restart.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Wait { get; set; }

        /// <summary>
        /// <para type="description">The timeout in seconds to wait for the VM to restart.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int Timeout { get; set; } = 120;

        /// <summary>
        /// <para type="description">Whether to force restart the VM.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        /// <summary>
        /// <para type="description">Whether to return the VM object after restarting.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter PassThru { get; set; }

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

                if (status != "running" && !Force.IsPresent)
                {
                    WriteError(new ErrorRecord(
                        new Exception($"VM with ID {VMID} is not running. Use -Force to start it."),
                        "VMNotRunning",
                        ErrorCategory.InvalidOperation,
                        VMID));
                    return;
                }

                // Confirm restart
                if (!ShouldProcess($"VM {VMID} on node {Node}", "Restart"))
                {
                    return;
                }

                if (status == "running")
                {
                    // Restart the VM
                    WriteVerbose($"Restarting VM {VMID} on node {Node}");
                    string restartUrl = $"nodes/{Node}/qemu/{VMID}/status/";
                    restartUrl += Force.IsPresent ? "reset" : "reboot";
                    client.Post(restartUrl, null);
                }
                else if (Force.IsPresent)
                {
                    // Start the VM
                    WriteVerbose($"Starting VM {VMID} on node {Node}");
                    client.Post($"nodes/{Node}/qemu/{VMID}/status/start", null);
                }

                // Wait for the VM to restart if requested
                if (Wait.IsPresent)
                {
                    WriteVerbose($"Waiting for VM {VMID} to restart");
                    int attempts = 0;
                    int maxAttempts = Timeout / 2;
                    bool restarted = false;

                    // First, wait for the VM to stop (if it was running)
                    if (status == "running")
                    {
                        while (attempts < maxAttempts / 2)
                        {
                            vmResponse = client.Get($"nodes/{Node}/qemu/{VMID}/status/current");
                            vmData = Newtonsoft.Json.Linq.JObject.Parse(vmResponse)["data"] as Newtonsoft.Json.Linq.JObject;
                            status = vmData["status"].ToString();

                            if (status != "running")
                            {
                                break;
                            }

                            System.Threading.Thread.Sleep(2000);
                            attempts++;
                        }
                    }

                    // Then, wait for the VM to start again
                    attempts = 0;
                    while (attempts < maxAttempts / 2)
                    {
                        vmResponse = client.Get($"nodes/{Node}/qemu/{VMID}/status/current");
                        vmData = Newtonsoft.Json.Linq.JObject.Parse(vmResponse)["data"] as Newtonsoft.Json.Linq.JObject;
                        status = vmData["status"].ToString();

                        if (status == "running")
                        {
                            restarted = true;
                            break;
                        }

                        System.Threading.Thread.Sleep(2000);
                        attempts++;
                    }

                    if (!restarted)
                    {
                        WriteWarning($"Timeout waiting for VM {VMID} to restart");
                    }
                    else
                    {
                        WriteVerbose($"VM {VMID} restarted successfully");
                    }
                }

                // Return the VM object if requested
                if (PassThru.IsPresent)
                {
                    vmResponse = client.Get($"nodes/{Node}/qemu/{VMID}/status/current");
                    var vm = PSProxmox.Utilities.JsonUtility.DeserializeResponse<ProxmoxVM>(vmResponse);
                    vm.Node = Node;
                    vm.VMID = VMID;
                    WriteObject(vm);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "RestartProxmoxVMError", ErrorCategory.OperationStopped, VMID));
            }
        }
    }
}
