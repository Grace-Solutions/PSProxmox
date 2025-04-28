using System;
using System.Management.Automation;
using PSProxmox.Client;
using PSProxmox.Models;
using PSProxmox.Session;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Stops a virtual machine in Proxmox VE.</para>
    /// <para type="description">The Stop-ProxmoxVM cmdlet stops a virtual machine in Proxmox VE.</para>
    /// <example>
    ///   <para>Stop a virtual machine</para>
    ///   <code>Stop-ProxmoxVM -Connection $connection -Node "pve1" -VMID 100</code>
    /// </example>
    /// <example>
    ///   <para>Stop a virtual machine using pipeline input</para>
    ///   <code>Get-ProxmoxVM -Connection $connection -VMID 100 | Stop-ProxmoxVM -Connection $connection</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsLifecycle.Stop, "ProxmoxVM", SupportsShouldProcess = true)]
    [OutputType(typeof(ProxmoxVM))]
    public class StopProxmoxVMCmdlet : PSCmdlet
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
        /// <para type="description">The ID of the VM to stop.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public int VMID { get; set; }

        /// <summary>
        /// <para type="description">Whether to wait for the VM to stop.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Wait { get; set; }

        /// <summary>
        /// <para type="description">The timeout in seconds to wait for the VM to stop.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int Timeout { get; set; } = 60;

        /// <summary>
        /// <para type="description">Whether to force stop the VM.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        /// <summary>
        /// <para type="description">Whether to return the VM object after stopping.</para>
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

                if (status != "running")
                {
                    WriteVerbose($"VM {VMID} is not running on node {Node}");
                    if (PassThru.IsPresent)
                    {
                        var vm = PSProxmox.Utilities.JsonUtility.DeserializeResponse<ProxmoxVM>(vmResponse);
                        vm.Node = Node;
                        vm.VMID = VMID;
                        WriteObject(vm);
                    }
                    return;
                }

                // Confirm stop
                if (!ShouldProcess($"VM {VMID} on node {Node}", "Stop"))
                {
                    return;
                }

                // Stop the VM
                WriteVerbose($"Stopping VM {VMID} on node {Node}");
                string stopUrl = $"nodes/{Node}/qemu/{VMID}/status/";
                stopUrl += Force.IsPresent ? "stop" : "shutdown";
                client.Post(stopUrl, null);

                // Wait for the VM to stop if requested
                if (Wait.IsPresent)
                {
                    WriteVerbose($"Waiting for VM {VMID} to stop");
                    int attempts = 0;
                    int maxAttempts = Timeout / 2;
                    bool stopped = false;

                    while (attempts < maxAttempts)
                    {
                        vmResponse = client.Get($"nodes/{Node}/qemu/{VMID}/status/current");
                        vmData = Newtonsoft.Json.Linq.JObject.Parse(vmResponse)["data"] as Newtonsoft.Json.Linq.JObject;
                        status = vmData["status"].ToString();

                        if (status != "running")
                        {
                            stopped = true;
                            break;
                        }

                        System.Threading.Thread.Sleep(2000);
                        attempts++;
                    }

                    if (!stopped)
                    {
                        WriteWarning($"Timeout waiting for VM {VMID} to stop");
                        if (Force.IsPresent)
                        {
                            WriteVerbose($"Forcing VM {VMID} to stop");
                            client.Post($"nodes/{Node}/qemu/{VMID}/status/stop", null);
                            System.Threading.Thread.Sleep(5000);
                        }
                    }
                    else
                    {
                        WriteVerbose($"VM {VMID} stopped successfully");
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
                WriteError(new ErrorRecord(ex, "StopProxmoxVMError", ErrorCategory.OperationStopped, VMID));
            }
        }
    }
}
