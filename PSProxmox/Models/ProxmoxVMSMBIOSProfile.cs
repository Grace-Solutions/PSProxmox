using System;
using System.Collections.Generic;
using System.Linq;

namespace PSProxmox.Models
{
    /// <summary>
    /// Represents a manufacturer profile for SMBIOS settings.
    /// </summary>
    public class ProxmoxVMSMBIOSProfile
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// Gets the available manufacturer profiles.
        /// </summary>
        public static readonly string[] AvailableProfiles = new string[]
        {
            "Proxmox",
            "Dell",
            "HP",
            "Lenovo",
            "VMware",
            "HyperV",
            "VirtualBox",
            "Random"
        };

        /// <summary>
        /// Gets a SMBIOS profile for the specified manufacturer.
        /// </summary>
        /// <param name="manufacturer">The manufacturer profile to use.</param>
        /// <returns>A ProxmoxVMSMBIOS object with the manufacturer profile settings.</returns>
        public static ProxmoxVMSMBIOS GetProfile(string manufacturer)
        {
            if (string.IsNullOrEmpty(manufacturer) || manufacturer.Equals("Random", StringComparison.OrdinalIgnoreCase))
            {
                // Choose a random manufacturer (excluding "Random")
                var profiles = AvailableProfiles.Where(p => !p.Equals("Random", StringComparison.OrdinalIgnoreCase)).ToArray();
                manufacturer = profiles[_random.Next(profiles.Length)];
            }

            switch (manufacturer.ToLowerInvariant())
            {
                case "proxmox":
                    return GetProxmoxProfile();
                case "dell":
                    return GetDellProfile();
                case "hp":
                    return GetHPProfile();
                case "lenovo":
                    return GetLenovoProfile();
                case "vmware":
                    return GetVMwareProfile();
                case "hyperv":
                    return GetHyperVProfile();
                case "virtualbox":
                    return GetVirtualBoxProfile();
                default:
                    throw new ArgumentException($"Unknown manufacturer profile: {manufacturer}");
            }
        }

        private static ProxmoxVMSMBIOS GetProxmoxProfile()
        {
            return new ProxmoxVMSMBIOS
            {
                Manufacturer = "Proxmox",
                Product = $"Virtual Environment {_random.Next(1, 8)}.{_random.Next(0, 10)}",
                Version = $"{_random.Next(1, 10)}.{_random.Next(0, 10)}",
                Serial = GenerateRandomSerial(10),
                UUID = GenerateRandomUUID(),
                Family = "Virtual Machine"
            };
        }

        private static ProxmoxVMSMBIOS GetDellProfile()
        {
            var models = new string[]
            {
                "PowerEdge R640",
                "PowerEdge R740",
                "PowerEdge R750",
                "PowerEdge R840",
                "PowerEdge R940",
                "PowerEdge T640"
            };

            return new ProxmoxVMSMBIOS
            {
                Manufacturer = "Dell Inc.",
                Product = models[_random.Next(models.Length)],
                Version = $"{_random.Next(1, 5)}.{_random.Next(0, 10)}",
                Serial = $"{GenerateRandomString(5, true)}{_random.Next(10000, 99999)}",
                UUID = GenerateRandomUUID(),
                Family = "PowerEdge"
            };
        }

        private static ProxmoxVMSMBIOS GetHPProfile()
        {
            var models = new string[]
            {
                "ProLiant DL360 Gen10",
                "ProLiant DL380 Gen10",
                "ProLiant DL560 Gen10",
                "ProLiant ML350 Gen10",
                "ProLiant BL460c Gen10"
            };

            return new ProxmoxVMSMBIOS
            {
                Manufacturer = "HP",
                Product = models[_random.Next(models.Length)],
                Version = $"U{_random.Next(10, 50)}",
                Serial = $"USE{_random.Next(100000, 999999)}",
                UUID = GenerateRandomUUID(),
                Family = "ProLiant"
            };
        }

        private static ProxmoxVMSMBIOS GetLenovoProfile()
        {
            var models = new string[]
            {
                "ThinkSystem SR650",
                "ThinkSystem SR630",
                "ThinkSystem SR850",
                "ThinkSystem SR950",
                "ThinkSystem ST550"
            };

            return new ProxmoxVMSMBIOS
            {
                Manufacturer = "Lenovo",
                Product = models[_random.Next(models.Length)],
                Version = $"ThinkSystem {_random.Next(1, 5)}",
                Serial = $"{GenerateRandomString(4, true)}{GenerateRandomString(6, false)}",
                UUID = GenerateRandomUUID(),
                Family = "ThinkSystem"
            };
        }

        private static ProxmoxVMSMBIOS GetVMwareProfile()
        {
            return new ProxmoxVMSMBIOS
            {
                Manufacturer = "VMware, Inc.",
                Product = $"VMware Virtual Platform {_random.Next(1, 8)}",
                Version = $"VMware-{_random.Next(1, 10)}.{_random.Next(0, 10)}",
                Serial = GenerateRandomSerial(10),
                UUID = GenerateRandomUUID(),
                Family = "Virtual Machine"
            };
        }

        private static ProxmoxVMSMBIOS GetHyperVProfile()
        {
            return new ProxmoxVMSMBIOS
            {
                Manufacturer = "Microsoft Corporation",
                Product = $"Hyper-V {_random.Next(2012, 2023)}",
                Version = $"{_random.Next(1, 10)}.{_random.Next(0, 10)}",
                Serial = $"MS-{GenerateRandomString(8, false)}",
                UUID = GenerateRandomUUID(),
                Family = "Virtual Machine"
            };
        }

        private static ProxmoxVMSMBIOS GetVirtualBoxProfile()
        {
            return new ProxmoxVMSMBIOS
            {
                Manufacturer = "Oracle Corporation",
                Product = $"VirtualBox {_random.Next(5, 8)}.{_random.Next(0, 10)}",
                Version = $"{_random.Next(1, 10)}.{_random.Next(0, 10)}",
                Serial = $"VB-{GenerateRandomString(8, false)}",
                UUID = GenerateRandomUUID(),
                Family = "Virtual Machine"
            };
        }

        private static string GenerateRandomSerial(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        private static string GenerateRandomString(int length, bool uppercase)
        {
            const string upperChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerChars = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";

            string chars = uppercase ? upperChars : lowerChars + digits;
            
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        private static string GenerateRandomUUID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
