using System;
using System.Management.Automation;
using PSProxmox.Client;
using PSProxmox.Models;
using PSProxmox.Session;
using PSProxmox.Templates;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Creates a new virtual machine template in Proxmox VE.</para>
    /// <para type="description">The New-ProxmoxVMTemplate cmdlet creates a new virtual machine template in Proxmox VE.</para>
    /// <example>
    ///   <para>Create a new template from an existing VM</para>
    ///   <code>$template = New-ProxmoxVMTemplate -Connection $connection -VMID 100 -Name "Ubuntu-Template" -Description "Ubuntu 20.04 Template"</code>
    /// </example>
    /// <example>
    ///   <para>Create a new template from an existing VM using pipeline input</para>
    ///   <code>Get-ProxmoxVM -Connection $connection -VMID 100 | New-ProxmoxVMTemplate -Connection $connection -Name "Ubuntu-Template" -Description "Ubuntu 20.04 Template"</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "ProxmoxVMTemplate")]
    [OutputType(typeof(ProxmoxVMTemplate))]
    public class NewProxmoxVMTemplateCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The ID of the VM to convert to a template.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ParameterSetName = "FromVM")]
        public int VMID { get; set; }

        /// <summary>
        /// <para type="description">The node where the VM is located.</para>
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, ParameterSetName = "FromVM")]
        public string Node { get; set; }

        /// <summary>
        /// <para type="description">The name of the template.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">The description of the template.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Description { get; set; }

        /// <summary>
        /// <para type="description">Tags for the template.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string[] Tags { get; set; }

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

                if (status == "running")
                {
                    WriteError(new ErrorRecord(
                        new Exception($"VM with ID {VMID} is running. Stop the VM before converting to a template."),
                        "VMRunning",
                        ErrorCategory.InvalidOperation,
                        VMID));
                    return;
                }

                // Convert the VM to a template
                WriteVerbose($"Converting VM {VMID} to template on node {Node}");
                client.Post($"nodes/{Node}/qemu/{VMID}/template", null);

                // Get the VM details
                vmResponse = client.Get($"nodes/{Node}/qemu/{VMID}/config");
                var vm = PSProxmox.Utilities.JsonUtility.DeserializeResponse<ProxmoxVM>(vmResponse);
                vm.Node = Node;
                vm.VMID = VMID;

                // Create the template object
                var template = ProxmoxVMTemplate.FromVM(vm);
                template.Name = Name;
                template.Description = Description;
                template.Tags = Tags != null ? string.Join(",", Tags) : null;

                // Save the template
                TemplateManager.CreateTemplate(template);

                WriteObject(template);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "NewProxmoxVMTemplateError", ErrorCategory.OperationStopped, VMID));
            }
        }
    }
}
