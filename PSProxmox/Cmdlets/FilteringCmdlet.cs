using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace PSProxmox.Cmdlets
{
    /// <summary>
    /// Base class for cmdlets that support filtering with wildcards and regex.
    /// </summary>
    public abstract class FilteringCmdlet : ProxmoxCmdlet
    {
        /// <summary>
        /// <para type="description">Use regular expressions for filtering.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter UseRegex { get; set; }

        /// <summary>
        /// Filters a collection of objects by a property value using wildcards or regex.
        /// </summary>
        /// <typeparam name="T">The type of objects to filter.</typeparam>
        /// <param name="items">The collection of objects to filter.</param>
        /// <param name="propertyName">The name of the property to filter by.</param>
        /// <param name="pattern">The pattern to match. Can be a wildcard or regex pattern.</param>
        /// <returns>A filtered collection of objects.</returns>
        protected IEnumerable<T> FilterByProperty<T>(IEnumerable<T> items, string propertyName, string pattern)
        {
            if (string.IsNullOrEmpty(pattern))
            {
                return items;
            }

            var filteredItems = new List<T>();
            var type = typeof(T);
            var property = type.GetProperty(propertyName);

            if (property == null)
            {
                throw new ArgumentException($"Property '{propertyName}' not found on type '{type.Name}'.");
            }

            foreach (var item in items)
            {
                var value = property.GetValue(item)?.ToString();
                if (value == null)
                {
                    continue;
                }

                bool isMatch;
                if (UseRegex.IsPresent)
                {
                    // Use regex matching
                    try
                    {
                        isMatch = Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase);
                    }
                    catch (ArgumentException ex)
                    {
                        throw new ArgumentException($"Invalid regular expression pattern: {ex.Message}", ex);
                    }
                }
                else
                {
                    // Use wildcard matching
                    isMatch = WildcardMatch(value, pattern);
                }

                if (isMatch)
                {
                    filteredItems.Add(item);
                }
            }

            return filteredItems;
        }

        /// <summary>
        /// Matches a string against a wildcard pattern.
        /// </summary>
        /// <param name="input">The input string to match.</param>
        /// <param name="pattern">The wildcard pattern to match against.</param>
        /// <returns>True if the input matches the pattern, false otherwise.</returns>
        private bool WildcardMatch(string input, string pattern)
        {
            // Convert wildcard pattern to regex pattern
            string regexPattern = "^" + Regex.Escape(pattern)
                .Replace("\\*", ".*")
                .Replace("\\?", ".") + "$";

            return Regex.IsMatch(input, regexPattern, RegexOptions.IgnoreCase);
        }
    }
}
