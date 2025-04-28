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
    /// <para type="synopsis">Gets cluster backups from Proxmox VE.</para>
    /// <para type="description">The Get-ProxmoxClusterBackup cmdlet retrieves cluster backups from Proxmox VE.</para>
    /// <example>
    ///   <para>Get all cluster backups</para>
    ///   <code>$backups = Get-ProxmoxClusterBackup -Connection $connection</code>
    /// </example>
    /// <example>
    ///   <para>Get a specific cluster backup by ID</para>
    ///   <code>$backup = Get-ProxmoxClusterBackup -Connection $connection -BackupID "vzdump-cluster-2023_04_28-12_00_00.vma.lzo"</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "ProxmoxClusterBackup")]
    [OutputType(typeof(ProxmoxClusterBackup), typeof(string))]
    public class GetProxmoxClusterBackupCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to the Proxmox VE server.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">The ID of the backup to retrieve.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public string BackupID { get; set; }

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

                response = client.Get("cluster/backup");
                var backupsData = JsonUtility.DeserializeResponse<JArray>(response);
                var backups = new List<ProxmoxClusterBackup>();

                foreach (var backupObj in backupsData)
                {
                    var backup = backupObj.ToObject<ProxmoxClusterBackup>();
                    
                    if (string.IsNullOrEmpty(BackupID) || backup.BackupID == BackupID)
                    {
                        backups.Add(backup);
                    }
                }

                if (RawJson.IsPresent)
                {
                    WriteObject(response);
                }
                else if (string.IsNullOrEmpty(BackupID))
                {
                    WriteObject(backups, true);
                }
                else if (backups.Count > 0)
                {
                    WriteObject(backups[0]);
                }
                else
                {
                    WriteError(new ErrorRecord(
                        new Exception($"Backup with ID {BackupID} not found"),
                        "BackupNotFound",
                        ErrorCategory.ObjectNotFound,
                        BackupID));
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetProxmoxClusterBackupError", ErrorCategory.OperationStopped, Connection));
            }
        }
    }
}
