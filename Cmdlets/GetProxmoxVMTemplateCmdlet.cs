using System;
using System.Collections.Generic;
using System.Management.Automation;
using PSProxmox.Models;
using PSProxmox.Templates;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// <para type="synopsis">Gets virtual machine templates.</para>
    /// <para type="description">The Get-ProxmoxVMTemplate cmdlet retrieves virtual machine templates.</para>
    /// <example>
    ///   <para>Get all templates</para>
    ///   <code>$templates = Get-ProxmoxVMTemplate</code>
    /// </example>
    /// <example>
    ///   <para>Get a specific template by name</para>
    ///   <code>$template = Get-ProxmoxVMTemplate -Name "Ubuntu-Template"</code>
    /// </example>
    /// </summary>
    [Cmdlet(VerbsCommon.Get, "ProxmoxVMTemplate")]
    [OutputType(typeof(ProxmoxVMTemplate))]
    public class GetProxmoxVMTemplateCmdlet : PSCmdlet
    {
        /// <summary>
        /// <para type="description">The name of the template to retrieve.</para>
        /// </summary>
        [Parameter(Mandatory = false, Position = 0)]
        public string Name { get; set; }

        /// <summary>
        /// Processes the cmdlet.
        /// </summary>
        protected override void ProcessRecord()
        {
            try
            {
                if (string.IsNullOrEmpty(Name))
                {
                    // Get all templates
                    var templates = new List<ProxmoxVMTemplate>();
                    foreach (var template in TemplateManager.GetTemplates())
                    {
                        templates.Add(template);
                    }
                    WriteObject(templates, true);
                }
                else
                {
                    // Get a specific template
                    var template = TemplateManager.GetTemplate(Name);
                    WriteObject(template);
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetProxmoxVMTemplateError", ErrorCategory.InvalidOperation, Name));
            }
        }
    }
}
