using System;
using System.Collections.Generic;
using System.Management.Automation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PSProxmox.Client;
using PSProxmox.Models;
using PSProxmox.Session;
using PSProxmox.Utilities;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Gets storage from a Proxmox VE server.</para>
    /// <para type="description">The Get-ProxmoxStorage cmdlet retrieves storage from a Proxmox VE server.</para>
    /// <example>
    ///   <para>Get all storage</para>
    ///   <code>$storage = Get-ProxmoxStorage -Connection $connection</code>
    /// </example>
    /// <example>
    ///   <para>Get a specific storage by name</para>
    ///   <code>$storage = Get-ProxmoxStorage -Connection $connection -Name "local"</code>
    /// </example>
    /// <example>
    ///   <para>Get storage on a specific node</para>
    ///   <code>$storage = Get-ProxmoxStorage -Connection $connection -Node "pve1"</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "ProxmoxStorage")]
    [OutputType(typeof(ProxmoxStorage), typeof(string))]
    public class GetProxmoxStorageCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The name of the storage to retrieve.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Name { get; set; }

        /// <summary>
        /// <para type="description">The node to retrieve storage from.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Node { get; set; }

        /// <summary>
        /// <para type="description">Whether to return the raw JSON response.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter RawJson { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);
                string response;

                if (string.IsNullOrEmpty(Node))
                {
                    // Get storage from all nodes
                    if (string.IsNullOrEmpty(Name))
                    {
                        // Get all storage
                        response = client.Get("storage");
                        var storageData = JsonUtility.DeserializeResponse<JArray>(response);
                        var allStorage = new List<ProxmoxStorage>();

                        foreach (var storageObj in storageData)
                        {
                            var storage = storageObj.ToObject<ProxmoxStorage>();
                            allStorage.Add(storage);
                        }

                        if (RawJson.IsPresent)
                        {
                            WriteObject(response);
                        }
                        else
                        {
                            WriteObject(allStorage, true);
                        }
                    }
                    else
                    {
                        // Get a specific storage
                        response = client.Get($"storage/{Name}");
                        var storage = JsonUtility.DeserializeResponse<ProxmoxStorage>(response);

                        if (RawJson.IsPresent)
                        {
                            WriteObject(response);
                        }
                        else
                        {
                            WriteObject(storage);
                        }
                    }
                }
                else
                {
                    // Get storage from a specific node
                    if (string.IsNullOrEmpty(Name))
                    {
                        // Get all storage on the node
                        response = client.Get($"nodes/{Node}/storage");
                        var storageData = JsonUtility.DeserializeResponse<JArray>(response);
                        var nodeStorage = new List<ProxmoxStorage>();

                        foreach (var storageObj in storageData)
                        {
                            var storage = storageObj.ToObject<ProxmoxStorage>();
                            storage.Node = Node;
                            nodeStorage.Add(storage);
                        }

                        if (RawJson.IsPresent)
                        {
                            WriteObject(response);
                        }
                        else
                        {
                            WriteObject(nodeStorage, true);
                        }
                    }
                    else
                    {
                        // Get a specific storage on the node
                        response = client.Get($"nodes/{Node}/storage/{Name}");
                        var storage = JsonUtility.DeserializeResponse<ProxmoxStorage>(response);
                        storage.Node = Node;

                        if (RawJson.IsPresent)
                        {
                            WriteObject(response);
                        }
                        else
                        {
                            WriteObject(storage);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetProxmoxStorageError", ErrorCategory.OperationStopped, Connection));
            }
        }
    }
}
