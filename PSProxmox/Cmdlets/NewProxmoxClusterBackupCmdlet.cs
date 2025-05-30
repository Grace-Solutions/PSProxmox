using System;
using System.Collections.Generic;
using System.Management.Automation;
using Newtonsoft.Json.Linq;
using PSProxmox.Client;
using PSProxmox.Models;
using PSProxmox.Session;
using PSProxmox.Utilities;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Creates a new cluster backup in Proxmox VE.</para>
    /// <para type="description">The New-ProxmoxClusterBackup cmdlet creates a new cluster backup in Proxmox VE.</para>
    /// <example>
    ///   <para>Create a new cluster backup</para>
    ///   <code>$backup = New-ProxmoxClusterBackup -Connection $connection -Compress</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.New, "ProxmoxClusterBackup")]
    [OutputType(typeof(ProxmoxClusterBackup))]
    public class NewProxmoxClusterBackupCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">Whether to compress the backup.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Compress { get; set; }

        /// <summary>
        /// <para type="description">Whether to wait for the backup to complete.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Wait { get; set; }

        /// <summary>
        /// <para type="description">The timeout in seconds to wait for the backup to complete.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int Timeout { get; set; } = 300;

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);

                // Create the backup
                var parameters = new Dictionary<string, string>();

                if (Compress.IsPresent)
                {
                    parameters["compress"] = "1";
                }

                WriteVerbose("Creating cluster backup");
                string response = client.Post("cluster/backup", parameters);
                var taskData = JsonUtility.DeserializeResponse<JObject>(response);
                string taskId = taskData["data"]?.ToString();

                if (Wait.IsPresent && !string.IsNullOrEmpty(taskId))
                {
                    WriteVerbose($"Waiting for backup task {taskId} to complete");
                    int attempts = 0;
                    int maxAttempts = Timeout / 5;
                    bool completed = false;

                    while (attempts < maxAttempts)
                    {
                        string taskResponse = client.Get($"nodes/{Connection.Server}/tasks/{taskId}/status");
                        var taskStatus = JsonUtility.DeserializeResponse<JObject>(taskResponse);
                        string status = taskStatus["data"]?["status"]?.ToString();

                        if (status == "stopped")
                        {
                            completed = true;
                            break;
                        }

                        System.Threading.Thread.Sleep(5000);
                        attempts++;
                    }

                    if (!completed)
                    {
                        WriteWarning($"Timeout waiting for backup task {taskId} to complete");
                    }
                    else
                    {
                        WriteVerbose($"Backup task {taskId} completed successfully");
                    }
                }

                // Get the latest backup
                string backupsResponse = client.Get("cluster/backup");
                var backupsData = JsonUtility.DeserializeResponse<JObject>(backupsResponse);
                var backups = backupsData["data"] as JArray;

                if (backups != null && backups.Count > 0)
                {
                    var latestBackup = backups[0] as JObject;
                    var backup = new ProxmoxClusterBackup
                    {
                        BackupID = latestBackup?["backup-id"]?.ToString(),
                        Time = long.TryParse(latestBackup?["time"]?.ToString(), out long time) ? time : 0,
                        Type = latestBackup?["type"]?.ToString(),
                        Version = latestBackup?["version"]?.ToString(),
                        Size = long.TryParse(latestBackup?["size"]?.ToString(), out long size) ? size : 0,
                        Compression = latestBackup?["compression"]?.ToString(),
                        Node = latestBackup?["node"]?.ToString(),
                        Path = latestBackup?["path"]?.ToString()
                    };

                    WriteObject(backup);
                }
                else
                {
                    WriteWarning("No backups found after creating backup");
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "NewProxmoxClusterBackupError", ErrorCategory.OperationStopped, Connection));
            }
        }
    }
}
