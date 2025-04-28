using System;
using System.Collections.Generic;
using System.Management.Automation;
using PSProxmox.Client;
using PSProxmox.Models;
using PSProxmox.Session;
using PSProxmox.Utilities;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Creates a new storage in Proxmox VE.</para>
    /// <para type="description">The New-ProxmoxStorage cmdlet creates a new storage in Proxmox VE.</para>
    /// <example>
    ///   <para>Create a new directory storage</para>
    ///   <code>$storage = New-ProxmoxStorage -Connection $connection -Name "backup" -Type "dir" -Path "/mnt/backup" -Content "backup,iso"</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "ProxmoxStorage")]
    [OutputType(typeof(ProxmoxStorage))]
    public class NewProxmoxStorageCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The name of the storage.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">The type of the storage.</para>
        /// </summary>
        [Parameter(Mandatory = true)]
        [ValidateSet("dir", "nfs", "cifs", "lvm", "lvmthin", "zfs", "zfspool", "iscsi", "glusterfs", "cephfs", "rbd")]
        public string Type { get; set; }

        /// <summary>
        /// <para type="description">The path of the storage.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Path { get; set; }

        /// <summary>
        /// <para type="description">The content types allowed on the storage.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Content { get; set; }

        /// <summary>
        /// <para type="description">The node to create the storage on.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Node { get; set; }

        /// <summary>
        /// <para type="description">Whether the storage is shared.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Shared { get; set; }

        /// <summary>
        /// <para type="description">Whether the storage is enabled.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Enabled { get; set; } = true;

        /// <summary>
        /// <para type="description">Additional parameters for the storage.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public Hashtable AdditionalParameters { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);

                // Create the storage
                var parameters = new Dictionary<string, string>
                {
                    ["storage"] = Name,
                    ["type"] = Type
                };

                if (!string.IsNullOrEmpty(Path))
                {
                    parameters["path"] = Path;
                }

                if (!string.IsNullOrEmpty(Content))
                {
                    parameters["content"] = Content;
                }

                if (!string.IsNullOrEmpty(Node))
                {
                    parameters["nodes"] = Node;
                }

                parameters["shared"] = Shared.IsPresent ? "1" : "0";
                parameters["disable"] = Enabled.IsPresent ? "0" : "1";

                // Add additional parameters
                if (AdditionalParameters != null)
                {
                    foreach (var key in AdditionalParameters.Keys)
                    {
                        parameters[key.ToString()] = AdditionalParameters[key].ToString();
                    }
                }

                // Create the storage
                string createResponse = client.Post("storage", parameters);

                // Get the created storage
                string storageResponse = client.Get($"storage/{Name}");
                var storage = JsonUtility.DeserializeResponse<ProxmoxStorage>(storageResponse);

                WriteObject(storage);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "NewProxmoxStorageError", ErrorCategory.OperationStopped, Connection));
            }
        }
    }
}
