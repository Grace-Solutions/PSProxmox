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
    /// <para type="synopsis">Gets nodes from a Proxmox VE cluster.</para>
    /// <para type="description">The Get-ProxmoxNode cmdlet retrieves nodes from a Proxmox VE cluster.</para>
    /// <example>
    ///   <para>Get all nodes</para>
    ///   <code>$nodes = Get-ProxmoxNode -Connection $connection</code>
    /// </example>
    /// <example>
    ///   <para>Get a specific node by name</para>
    ///   <code>$node = Get-ProxmoxNode -Connection $connection -Name "pve1"</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "ProxmoxNode")]
    [OutputType(typeof(ProxmoxNode), typeof(string))]
    public class GetProxmoxNodeCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The name of the node to retrieve.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string Name { get; set; }

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

                if (string.IsNullOrEmpty(Name))
                {
                    // Get all nodes
                    response = client.Get("nodes");
                    var nodesData = JsonUtility.DeserializeResponse<JArray>(response);
                    var nodes = new List<ProxmoxNode>();

                    foreach (var nodeObj in nodesData)
                    {
                        var node = nodeObj.ToObject<ProxmoxNode>();
                        nodes.Add(node);
                    }

                    if (RawJson.IsPresent)
                    {
                        WriteObject(response);
                    }
                    else
                    {
                        WriteObject(nodes, true);
                    }
                }
                else
                {
                    // Get a specific node
                    response = client.Get($"nodes/{Name}/status");
                    var nodeData = JsonUtility.DeserializeResponse<JObject>(response);
                    var node = nodeData.ToObject<ProxmoxNode>();
                    node.Name = Name;

                    if (RawJson.IsPresent)
                    {
                        WriteObject(response);
                    }
                    else
                    {
                        WriteObject(node);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetProxmoxNodeError", ErrorCategory.OperationStopped, Connection));
            }
        }
    }
}
