using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Security;
using Newtonsoft.Json.Linq;
using PSProxmox.Client;
using PSProxmox.Models;
using PSProxmox.Session;
using PSProxmox.Utilities;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Creates a new Proxmox LXC container</para>
    /// <para type="description">Creates a new Proxmox LXC container on a Proxmox server</para>
    /// </summary>
    /// <example>
    /// <code>New-ProxmoxContainer -Node "pve1" -Name "web-container" -OSTemplate "local:vztmpl/ubuntu-20.04-standard_20.04-1_amd64.tar.gz" -Storage "local-lvm" -Memory 512 -Swap 512 -Cores 1 -DiskSize 8</code>
    /// <para>Creates a new LXC container on node pve1</para>
    /// </example>
    /// <example>
    /// <code>$builder = New-ProxmoxContainerBuilder -Name "web-container"
    /// $builder.WithOSTemplate("local:vztmpl/ubuntu-20.04-standard_20.04-1_amd64.tar.gz")
    ///         .WithStorage("local-lvm")
    ///         .WithMemory(512)
    ///         .WithSwap(512)
    ///         .WithCores(1)
    ///         .WithDiskSize(8)
    ///         .WithUnprivileged($true)
    ///         .WithStartOnBoot($true)
    ///         .WithStart($true)
    /// New-ProxmoxContainer -Node "pve1" -Builder $builder</code>
    /// <para>Creates a new LXC container on node pve1 using a builder</para>
    /// </example>
    [Cmdlet(VerbsCommon.New, "ProxmoxContainer")]
    [OutputType(typeof(ProxmoxContainer))]
    public class NewProxmoxContainerCmdlet : ProxmoxCmdlet
    {
        /// <summary>
        /// Gets or sets the node name
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        public string Node { get; set; }

        /// <summary>
        /// Gets or sets the container builder
        /// </summary>
        [Parameter(Mandatory = false, Position = 1, ValueFromPipeline = true)]
        public ProxmoxContainerBuilder Builder { get; set; }

        /// <summary>
        /// Gets or sets the container ID
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public int? CTID { get; set; }

        /// <summary>
        /// Gets or sets the container name
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the container OS template
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public string OSTemplate { get; set; }

        /// <summary>
        /// Gets or sets the container storage
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public string Storage { get; set; }

        /// <summary>
        /// Gets or sets the container memory limit in MB
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public int? Memory { get; set; }

        /// <summary>
        /// Gets or sets the container swap limit in MB
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public int? Swap { get; set; }

        /// <summary>
        /// Gets or sets the container CPU cores
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public int? Cores { get; set; }

        /// <summary>
        /// Gets or sets the container disk size in GB
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public int? DiskSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the container is unprivileged
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public SwitchParameter Unprivileged { get; set; }

        /// <summary>
        /// Gets or sets the container password
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public SecureString Password { get; set; }

        /// <summary>
        /// Gets or sets the container SSH public key
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public string SSHKey { get; set; }

        /// <summary>
        /// Gets or sets the container description
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to start the container on boot
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public SwitchParameter StartOnBoot { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to start the container after creation
        /// </summary>
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public SwitchParameter Start { get; set; }

        /// <summary>
        /// Processes the cmdlet
        /// </summary>
        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            try
            {
                var client = GetProxmoxClient();
                Dictionary<string, string> parameters;

                if (Builder != null)
                {
                    // Convert builder parameters to string dictionary
                    parameters = new Dictionary<string, string>();
                    foreach (var kvp in Builder.Parameters)
                    {
                        parameters[kvp.Key] = kvp.Value?.ToString() ?? "";
                    }
                }
                else
                {
                    // Build parameters from individual properties
                    parameters = new Dictionary<string, string>();

                    if (!string.IsNullOrEmpty(Name))
                    {
                        parameters["hostname"] = Name;
                    }

                    if (!string.IsNullOrEmpty(OSTemplate))
                    {
                        parameters["ostemplate"] = OSTemplate;
                    }

                    if (!string.IsNullOrEmpty(Storage))
                    {
                        parameters["storage"] = Storage;
                    }

                    if (Memory.HasValue)
                    {
                        parameters["memory"] = Memory.Value.ToString();
                    }

                    if (Swap.HasValue)
                    {
                        parameters["swap"] = Swap.Value.ToString();
                    }

                    if (Cores.HasValue)
                    {
                        parameters["cores"] = Cores.Value.ToString();
                    }

                    if (DiskSize.HasValue && !string.IsNullOrEmpty(Storage))
                    {
                        parameters["rootfs"] = $"{Storage}:{DiskSize.Value}";
                    }

                    if (Unprivileged.IsPresent)
                    {
                        parameters["unprivileged"] = "1";
                    }

                    if (Password != null)
                    {
                        parameters["password"] = ConvertSecureStringToString(Password);
                    }

                    if (!string.IsNullOrEmpty(SSHKey))
                    {
                        parameters["ssh-public-keys"] = SSHKey;
                    }

                    if (!string.IsNullOrEmpty(Description))
                    {
                        parameters["description"] = Description;
                    }

                    if (StartOnBoot.IsPresent)
                    {
                        parameters["onboot"] = "1";
                    }

                    if (Start.IsPresent)
                    {
                        parameters["start"] = "1";
                    }
                }

                // Add CTID if specified
                if (CTID.HasValue)
                {
                    parameters["vmid"] = CTID.Value.ToString();
                }

                // Create the container
                var response = client.Post($"nodes/{Node}/lxc", parameters);
                var responseData = JsonUtility.DeserializeResponse<JObject>(response);
                var ctid = int.Parse(responseData["vmid"]?.ToString() ?? "0");

                // Wait for the task to complete
                var taskId = responseData["upid"]?.ToString();
                var taskStatus = WaitForTask(Node, taskId);

                if (taskStatus != "OK")
                {
                    throw new Exception($"Failed to create container: {taskStatus}");
                }

                // Get the created container
                var container = GetContainer(Node, ctid);
                WriteObject(container);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "NewProxmoxContainerError", ErrorCategory.OperationStopped, null));
            }
        }

        private string WaitForTask(string node, string taskId)
        {
            var client = GetProxmoxClient();
            var status = "";
            var attempts = 0;
            var maxAttempts = 60; // Wait up to 60 seconds

            while (attempts < maxAttempts)
            {
                var response = client.Get($"nodes/{node}/tasks/{taskId}/status");
                var data = JsonUtility.DeserializeResponse<JObject>(response);
                status = data["status"]?.ToString();

                if (status == "stopped")
                {
                    return data["exitstatus"]?.ToString() ?? "OK";
                }

                System.Threading.Thread.Sleep(1000);
                attempts++;
            }

            throw new Exception("Timeout waiting for task to complete");
        }

        private ProxmoxContainer GetContainer(string node, int ctid)
        {
            var client = GetProxmoxClient();
            var response = client.Get($"nodes/{node}/lxc/{ctid}/status/current");
            var container = JsonUtility.DeserializeResponse<ProxmoxContainer>(response);

            container.Node = node;
            container.CTID = ctid;

            return container;
        }

        private string ConvertSecureStringToString(SecureString secureString)
        {
            var ptr = System.Runtime.InteropServices.Marshal.SecureStringToBSTR(secureString);
            try
            {
                return System.Runtime.InteropServices.Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                System.Runtime.InteropServices.Marshal.ZeroFreeBSTR(ptr);
            }
        }
    }
}
