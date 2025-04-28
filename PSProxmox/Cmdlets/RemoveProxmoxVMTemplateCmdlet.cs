using System;
using System.Management.Automation;
using PSProxmox.Models;
using PSProxmox.Templates;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Removes a virtual machine template.</para>
    /// <para type="description">The Remove-ProxmoxVMTemplate cmdlet removes a virtual machine template.</para>
    /// <example>
    ///   <para>Remove a template</para>
    ///   <code>Remove-ProxmoxVMTemplate -Name "Ubuntu-Template"</code>
    /// </example>
    /// <example>
    ///   <para>Remove a template using pipeline input</para>
    ///   <code>Get-ProxmoxVMTemplate -Name "Ubuntu-Template" | Remove-ProxmoxVMTemplate</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Remove, "ProxmoxVMTemplate", SupportsShouldProcess = true)]
    public class RemoveProxmoxVMTemplateCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The name of the template to remove.</para>
        /// </summary>
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = true)]
        public string Name { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                // Confirm removal
                if (!ShouldProcess($"Template {Name}", "Remove"))
                {
                    return;
                }

                // Remove the template
                WriteVerbose($"Removing template {Name}");
                TemplateManager.RemoveTemplate(Name);

                WriteVerbose($"Template {Name} removed");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "RemoveProxmoxVMTemplateError", ErrorCategory.InvalidOperation, Name));
            }
        }
    }
}
