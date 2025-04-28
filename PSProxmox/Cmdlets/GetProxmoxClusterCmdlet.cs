using System;
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
    /// <para type="synopsis">Gets cluster information from Proxmox VE.</para>
    /// <para type="description">The Get-ProxmoxCluster cmdlet retrieves cluster information from Proxmox VE.</para>
    /// <example>
    ///   <para>Get cluster information</para>
    ///   <code>$cluster = Get-ProxmoxCluster -Connection $connection</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "ProxmoxCluster")]
    [OutputType(typeof(ProxmoxCluster), typeof(string))]
    public class GetProxmoxClusterCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

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

                // Get cluster status
                string statusResponse = client.Get("cluster/status");
                var statusData = JsonUtility.DeserializeResponse<JArray>(statusResponse);

                // Get cluster config
                string configResponse = client.Get("cluster/config");
                var configData = JsonUtility.DeserializeResponse<JObject>(configResponse);

                // Create the cluster object
                var cluster = new ProxmoxCluster();

                // Parse the cluster name and ID from the config
                if (configData["cluster"] != null)
                {
                    cluster.Name = configData["cluster"]["name"]?.ToString();
                    cluster.ID = configData["cluster"]["clusterId"]?.ToString();
                }

                // Parse the cluster version
                if (statusData != null)
                {
                    foreach (var item in statusData)
                    {
                        if (item["type"]?.ToString() == "cluster")
                        {
                            cluster.Version = item["version"]?.ToString();
                            cluster.ConfigVersion = item["config_version"]?.ToObject<int>() ?? 0;
                            break;
                        }
                    }
                }

                if (RawJson.IsPresent)
                {
                    var combinedResponse = new JObject
                    {
                        ["status"] = statusData,
                        ["config"] = configData
                    };
                    WriteObject(combinedResponse.ToString());
                }
                else
                {
                    WriteObject(cluster);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetProxmoxClusterError", ErrorCategory.OperationStopped, Connection));
            }
        }
    }
}
