using System;
using System.Management.Automation;
using PSProxmox.Client;
using PSProxmox.Session;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Tests a connection to a Proxmox VE server.</para>
    /// <para type="description">The Test-ProxmoxConnection cmdlet tests if a connection to a Proxmox VE server is valid and active.</para>
    /// <example>
    ///   <para>Test the current connection</para>
    ///   <code>Test-ProxmoxConnection</code>
    /// </example>
    /// <example>
    ///   <para>Test a specific connection</para>
    ///   <code>Test-ProxmoxConnection -Connection $connection</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsDiagnostic.Test, "ProxmoxConnection")]
    [OutputType(typeof(bool))]
    public class TestProxmoxConnectionCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The connection to test. If not specified, the current connection will be used.</para>
        /// </summary>
        [Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true)]
        public ProxmoxConnection Connection { get; set; }

        /// <summary>
        /// <para type="description">Return detailed connection information instead of a boolean.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Detailed { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                // Get the connection from the global variable if not specified
                if (Connection == null)
                {
                    var globalConnection = SessionState.PSVariable.GetValue("DefaultProxmoxConnection") as ProxmoxConnection;
                    if (globalConnection == null)
                    {
                        WriteError(new ErrorRecord(
                            new Exception("No connection specified and no default connection found. Use Connect-ProxmoxServer first."),
                            "NoConnectionFound",
                            ErrorCategory.ConnectionError,
                            null));
                        return;
                    }
                    Connection = globalConnection;
                }

                // Test the connection by making a simple API call
                try
                {
                    var client = new ProxmoxApiClient(Connection, this);
                    string response = client.Get("version");
                    
                    if (Detailed.IsPresent)
                    {
                        // Return detailed connection information
                        var connectionInfo = new PSObject();
                        connectionInfo.Properties.Add(new PSNoteProperty("Server", Connection.Server));
                        connectionInfo.Properties.Add(new PSNoteProperty("Port", Connection.Port));
                        connectionInfo.Properties.Add(new PSNoteProperty("Username", Connection.Username));
                        connectionInfo.Properties.Add(new PSNoteProperty("Realm", Connection.Realm));
                        connectionInfo.Properties.Add(new PSNoteProperty("UseSSL", Connection.UseSSL));
                        connectionInfo.Properties.Add(new PSNoteProperty("IsAuthenticated", Connection.IsAuthenticated));
                        connectionInfo.Properties.Add(new PSNoteProperty("Status", "Connected"));
                        WriteObject(connectionInfo);
                    }
                    else
                    {
                        // Return a simple boolean
                        WriteObject(true);
                    }
                }
                catch (Exception ex)
                {
                    if (Detailed.IsPresent)
                    {
                        // Return detailed connection information with error
                        var connectionInfo = new PSObject();
                        connectionInfo.Properties.Add(new PSNoteProperty("Server", Connection.Server));
                        connectionInfo.Properties.Add(new PSNoteProperty("Port", Connection.Port));
                        connectionInfo.Properties.Add(new PSNoteProperty("Username", Connection.Username));
                        connectionInfo.Properties.Add(new PSNoteProperty("Realm", Connection.Realm));
                        connectionInfo.Properties.Add(new PSNoteProperty("UseSSL", Connection.UseSSL));
                        connectionInfo.Properties.Add(new PSNoteProperty("IsAuthenticated", Connection.IsAuthenticated));
                        connectionInfo.Properties.Add(new PSNoteProperty("Status", "Error"));
                        connectionInfo.Properties.Add(new PSNoteProperty("Error", ex.Message));
                        WriteObject(connectionInfo);
                    }
                    else
                    {
                        // Return a simple boolean
                        WriteObject(false);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "TestProxmoxConnectionError", ErrorCategory.ConnectionError, Connection));
            }
        }
    }
}
