using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using PSProxmox.Models;

namespace PSProxmox.Templates
{
    /// <summary>
    /// Manages VM templates for Proxmox VE.
    /// </summary>
    public class TemplateManager
    {
        private static readonly string TemplateDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "PSProxmox",
            "Templates");

        private static readonly Dictionary<string, ProxmoxVMTemplate> _templates = new Dictionary<string, ProxmoxVMTemplate>();
        private static bool _initialized = false;

        /// <summary>
        /// Initializes the template manager.
        /// </summary>
        public static void Initialize()
        {
            if (_initialized)
            {
                return;
            }

            if (!Directory.Exists(TemplateDirectory))
            {
                Directory.CreateDirectory(TemplateDirectory);
            }

            LoadTemplates();
            _initialized = true;
        }

        /// <summary>
        /// Loads templates from disk.
        /// </summary>
        private static void LoadTemplates()
        {
            _templates.Clear();

            foreach (var file in Directory.GetFiles(TemplateDirectory, "*.json"))
            {
                try
                {
                    var template = JsonConvert.DeserializeObject<ProxmoxVMTemplate>(File.ReadAllText(file));
                    _templates[template.Name] = template;
                }
                catch
                {
                    // Ignore invalid template files
                }
            }
        }

        /// <summary>
        /// Saves a template to disk.
        /// </summary>
        /// <param name="template">The template to save.</param>
        private static void SaveTemplate(ProxmoxVMTemplate template)
        {
            string filePath = Path.Combine(TemplateDirectory, $"{template.Name}.json");
            File.WriteAllText(filePath, JsonConvert.SerializeObject(template, Formatting.Indented));
        }

        /// <summary>
        /// Creates a new template.
        /// </summary>
        /// <param name="template">The template to create.</param>
        /// <returns>The created template.</returns>
        public static ProxmoxVMTemplate CreateTemplate(ProxmoxVMTemplate template)
        {
            Initialize();

            if (_templates.ContainsKey(template.Name))
            {
                throw new ArgumentException($"Template with name '{template.Name}' already exists");
            }

            _templates[template.Name] = template;
            SaveTemplate(template);
            return template;
        }

        /// <summary>
        /// Gets a template by name.
        /// </summary>
        /// <param name="name">The name of the template.</param>
        /// <returns>The template.</returns>
        public static ProxmoxVMTemplate GetTemplate(string name)
        {
            Initialize();

            if (!_templates.TryGetValue(name, out var template))
            {
                throw new KeyNotFoundException($"Template with name '{name}' not found");
            }

            return template;
        }

        /// <summary>
        /// Gets all templates.
        /// </summary>
        /// <returns>All templates.</returns>
        public static IEnumerable<ProxmoxVMTemplate> GetTemplates()
        {
            Initialize();
            return _templates.Values;
        }

        /// <summary>
        /// Removes a template.
        /// </summary>
        /// <param name="name">The name of the template to remove.</param>
        public static void RemoveTemplate(string name)
        {
            Initialize();

            if (!_templates.ContainsKey(name))
            {
                throw new KeyNotFoundException($"Template with name '{name}' not found");
            }

            _templates.Remove(name);
            string filePath = Path.Combine(TemplateDirectory, $"{name}.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// Updates a template.
        /// </summary>
        /// <param name="template">The template to update.</param>
        /// <returns>The updated template.</returns>
        public static ProxmoxVMTemplate UpdateTemplate(ProxmoxVMTemplate template)
        {
            Initialize();

            if (!_templates.ContainsKey(template.Name))
            {
                throw new KeyNotFoundException($"Template with name '{template.Name}' not found");
            }

            _templates[template.Name] = template;
            SaveTemplate(template);
            return template;
        }
    }
}
