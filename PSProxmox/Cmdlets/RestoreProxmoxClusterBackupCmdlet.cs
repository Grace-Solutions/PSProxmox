using System;
using System.Collections.Generic;
using System.Management.Automation;
using Newtonsoft.Json.Linq;
using PSProxmox.Client;
using PSProxmox.Models;
using PSProxmox.Session;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Restores a cluster backup in Proxmox VE.</para>
    /// <para type="description">The Restore-ProxmoxClusterBackup cmdlet restores a cluster backup in Proxmox VE.</para>
    /// <example>
    ///   <para>Restore a cluster backup</para>
    ///   <code>Restore-ProxmoxClusterBackup -Connection $connection -BackupID "vzdump-cluster-2023_04_28-12_00_00.vma.lzo" -Force</code>
    /// </example>
    /// <example>
    ///   <para>Restore a cluster backup using pipeline input</para>
    ///   <code>Get-ProxmoxClusterBackup -Connection $connection | Select-Object -First 1 | Restore-ProxmoxClusterBackup -Connection $connection -Force</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsData.Restore, "ProxmoxClusterBackup", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RestoreProxmoxClusterBackupCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The ID of the backup to restore.</para>
        /// </summary>
        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
        public string BackupID { get; set; }

        /// <summary>
        /// <para type="description">Whether to force the restore operation.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

        /// <summary>
        /// <para type="description">Whether to wait for the restore to complete.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Wait { get; set; }

        /// <summary>
        /// <para type="description">The timeout in seconds to wait for the restore to complete.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public int Timeout { get; set; } = 600;

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                var client = new ProxmoxApiClient(Connection, this);

                // Confirm restore
                if (!ShouldProcess($"Cluster backup {BackupID}", "Restore"))
                {
                    return;
                }

                // Restore the backup
                var parameters = new Dictionary<string, string>
                {
                    ["backup-id"] = BackupID
                };

                if (Force.IsPresent)
                {
                    parameters["force"] = "1";
                }

                WriteVerbose($"Restoring cluster backup {BackupID}");
                string response = client.Post("cluster/backup/restore", parameters);
                var taskData = PSProxmox.Utilities.JsonUtility.DeserializeResponse<JObject>(response);
                string taskId = taskData["data"]?.ToString();

                if (Wait.IsPresent && !string.IsNullOrEmpty(taskId))
                {
                    WriteVerbose($"Waiting for restore task {taskId} to complete");
                    int attempts = 0;
                    int maxAttempts = Timeout / 5;
                    bool completed = false;

                    while (attempts < maxAttempts)
                    {
                        string taskResponse = client.Get($"nodes/{Connection.Server}/tasks/{taskId}/status");
                        var taskStatus = PSProxmox.Utilities.JsonUtility.DeserializeResponse<JObject>(taskResponse);
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
                        WriteWarning($"Timeout waiting for restore task {taskId} to complete");
                    }
                    else
                    {
                        WriteVerbose($"Restore task {taskId} completed successfully");
                    }
                }

                WriteVerbose($"Cluster backup {BackupID} restored");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "RestoreProxmoxClusterBackupError", ErrorCategory.OperationStopped, BackupID));
            }
        }
    }
}
