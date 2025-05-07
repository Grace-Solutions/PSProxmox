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
            "Microsoft",
            "VMware",
            "HyperV",
            "VirtualBox",
            "Random"
        };

        /// <summary>
        /// Gets a SMBIOS profile for the specified manufacturer.
        /// </summary>
        /// <param name="manufacturer">The manufacturer profile to use.</param>
        /// <param name="existingSmbios">Optional existing SMBIOS settings to preserve values from.</param>
        /// <returns>A ProxmoxVMSMBIOS object with the manufacturer profile settings.</returns>
        public static ProxmoxVMSMBIOS GetProfile(string manufacturer, ProxmoxVMSMBIOS existingSmbios = null)
        {
            if (string.IsNullOrEmpty(manufacturer) || manufacturer.Equals("Random", StringComparison.OrdinalIgnoreCase))
            {
                // Choose a random manufacturer (excluding "Random")
                var profiles = AvailableProfiles.Where(p => !p.Equals("Random", StringComparison.OrdinalIgnoreCase)).ToArray();
                manufacturer = profiles[_random.Next(profiles.Length)];
            }

            ProxmoxVMSMBIOS result;
            switch (manufacturer.ToLowerInvariant())
            {
                case "proxmox":
                    result = GetProxmoxProfile();
                    break;
                case "dell":
                    result = GetDellProfile();
                    break;
                case "hp":
                    result = GetHPProfile();
                    break;
                case "lenovo":
                    result = GetLenovoProfile();
                    break;
                case "microsoft":
                    result = GetMicrosoftProfile();
                    break;
                case "vmware":
                    result = GetVMwareProfile();
                    break;
                case "hyperv":
                    result = GetHyperVProfile();
                    break;
                case "virtualbox":
                    result = GetVirtualBoxProfile();
                    break;
                default:
                    throw new ArgumentException($"Unknown manufacturer profile: {manufacturer}");
            }

            // Preserve UUID if it exists in the original SMBIOS settings
            if (existingSmbios != null && !string.IsNullOrEmpty(existingSmbios.UUID))
            {
                result.UUID = existingSmbios.UUID;
            }

            return result;
        }

        private static ProxmoxVMSMBIOS GetProxmoxProfile()
        {
            var versions = new string[]
            {
                "6.2",
                "6.4",
                "7.0",
                "7.1",
                "7.2",
                "7.3",
                "7.4",
                "8.0"
            };

            string version = versions[_random.Next(versions.Length)];
            string buildNumber = $"{_random.Next(1, 20)}";

            return new ProxmoxVMSMBIOS
            {
                Manufacturer = "Proxmox",
                Product = $"Virtual Environment {version}",
                Version = $"{version}-{buildNumber}",
                Serial = $"PVE-{GenerateRandomString(8, false)}",
                UUID = GenerateRandomUUID(),
                Family = "Virtual Machine"
            };
        }

        private static ProxmoxVMSMBIOS GetDellProfile()
        {
            // Decide between server or workstation
            bool isServer = _random.Next(2) == 0;

            string product;
            string family;
            string serial;

            if (isServer)
            {
                var serverModels = new string[]
                {
                    "PowerEdge R640",
                    "PowerEdge R740",
                    "PowerEdge R750",
                    "PowerEdge R840",
                    "PowerEdge R940",
                    "PowerEdge T640",
                    "PowerEdge R440",
                    "PowerEdge R540",
                    "PowerEdge R650",
                    "PowerEdge R7525",
                    "PowerEdge C6420"
                };

                product = serverModels[_random.Next(serverModels.Length)];
                family = "PowerEdge";

                // Dell server serial format: typically 7 characters, service tag style
                serial = GenerateRandomString(7, true);
            }
            else
            {
                var workstationModels = new string[]
                {
                    "Precision 5820 Tower",
                    "Precision 7920 Tower",
                    "Precision 7820 Tower",
                    "Precision 3650 Tower",
                    "Precision 3450 SFF",
                    "Precision 5820 Rack",
                    "OptiPlex 7090",
                    "OptiPlex 5090",
                    "Latitude 7420",
                    "Latitude 5420",
                    "XPS 15 9510"
                };

                product = workstationModels[_random.Next(workstationModels.Length)];
                family = product.StartsWith("Precision") ? "Precision" :
                         product.StartsWith("OptiPlex") ? "OptiPlex" :
                         product.StartsWith("Latitude") ? "Latitude" : "XPS";

                // Dell workstation serial format
                serial = GenerateRandomString(7, true);
            }

            return new ProxmoxVMSMBIOS
            {
                Manufacturer = "Dell Inc.",
                Product = product,
                Version = $"{_random.Next(1, 5)}.{_random.Next(0, 10)}",
                Serial = serial,
                UUID = GenerateRandomUUID(),
                Family = family
            };
        }

        private static ProxmoxVMSMBIOS GetHPProfile()
        {
            // Decide between server or workstation
            bool isServer = _random.Next(2) == 0;

            string product;
            string family;
            string serial;
            string manufacturer = _random.Next(2) == 0 ? "HP" : "Hewlett-Packard";

            if (isServer)
            {
                var serverModels = new string[]
                {
                    "ProLiant DL360 Gen10",
                    "ProLiant DL380 Gen10",
                    "ProLiant DL560 Gen10",
                    "ProLiant ML350 Gen10",
                    "ProLiant BL460c Gen10",
                    "ProLiant DL380 Gen11",
                    "ProLiant DL360 Gen11",
                    "ProLiant ML30 Gen10 Plus",
                    "ProLiant DL20 Gen10",
                    "ProLiant DL325 Gen10 Plus",
                    "ProLiant DL385 Gen10 Plus"
                };

                product = serverModels[_random.Next(serverModels.Length)];
                family = "ProLiant";

                // HP server serial format
                serial = $"USE{_random.Next(100000, 999999)}";
            }
            else
            {
                var workstationModels = new string[]
                {
                    "Z4 G4 Workstation",
                    "Z6 G4 Workstation",
                    "Z8 G4 Workstation",
                    "Z2 Tower G8 Workstation",
                    "Z2 Mini G5 Workstation",
                    "EliteDesk 800 G6",
                    "EliteBook 840 G8",
                    "EliteBook 850 G8",
                    "ZBook Fury 15 G8",
                    "ZBook Fury 17 G8",
                    "ZBook Studio G8"
                };

                product = workstationModels[_random.Next(workstationModels.Length)];

                if (product.StartsWith("Z") && !product.StartsWith("ZBook"))
                {
                    family = "Z Workstation";
                }
                else if (product.StartsWith("ZBook"))
                {
                    family = "ZBook";
                }
                else if (product.StartsWith("EliteDesk"))
                {
                    family = "EliteDesk";
                }
                else
                {
                    family = "EliteBook";
                }

                // HP workstation serial format - typically 10 characters for newer models
                serial = $"CZ{_random.Next(10000000, 99999999)}";
            }

            return new ProxmoxVMSMBIOS
            {
                Manufacturer = manufacturer,
                Product = product,
                Version = $"U{_random.Next(10, 50)}",
                Serial = serial,
                UUID = GenerateRandomUUID(),
                Family = family
            };
        }

        private static ProxmoxVMSMBIOS GetLenovoProfile()
        {
            // Decide between server or workstation
            bool isServer = _random.Next(2) == 0;

            string product;
            string family;
            string serial;

            if (isServer)
            {
                var serverModels = new string[]
                {
                    "ThinkSystem SR650",
                    "ThinkSystem SR630",
                    "ThinkSystem SR850",
                    "ThinkSystem SR950",
                    "ThinkSystem ST550",
                    "ThinkSystem SR670 V2",
                    "ThinkSystem SR650 V2",
                    "ThinkSystem SR630 V2",
                    "ThinkSystem SR860 V2",
                    "ThinkSystem SD530",
                    "ThinkSystem SR550"
                };

                product = serverModels[_random.Next(serverModels.Length)];
                family = "ThinkSystem";

                // Lenovo server serial format
                serial = $"{GenerateRandomString(4, true)}{GenerateRandomString(6, false)}";
            }
            else
            {
                var workstationModels = new string[]
                {
                    "ThinkStation P620",
                    "ThinkStation P520",
                    "ThinkStation P720",
                    "ThinkStation P920",
                    "ThinkStation P340 Tiny",
                    "ThinkStation P340 Tower",
                    "ThinkPad P15 Gen 2",
                    "ThinkPad P17 Gen 2",
                    "ThinkPad P1 Gen 4",
                    "ThinkPad X1 Carbon Gen 9",
                    "ThinkPad T14 Gen 2"
                };

                product = workstationModels[_random.Next(workstationModels.Length)];

                if (product.StartsWith("ThinkStation"))
                {
                    family = "ThinkStation";
                }
                else
                {
                    family = "ThinkPad";
                }

                // Lenovo workstation serial format
                if (family == "ThinkStation")
                {
                    // ThinkStation format
                    serial = $"S{GenerateRandomString(2, true)}{GenerateRandomString(7, false)}";
                }
                else
                {
                    // ThinkPad format
                    serial = $"R{_random.Next(0, 9)}{GenerateRandomString(2, true)}{GenerateRandomString(6, false)}";
                }
            }

            return new ProxmoxVMSMBIOS
            {
                Manufacturer = "Lenovo",
                Product = product,
                Version = family == "ThinkSystem" ? $"ThinkSystem {_random.Next(1, 5)}" :
                          family == "ThinkStation" ? $"ThinkStation {_random.Next(1, 5)}" :
                          $"ThinkPad {_random.Next(1, 5)}",
                Serial = serial,
                UUID = GenerateRandomUUID(),
                Family = family
            };
        }

        private static ProxmoxVMSMBIOS GetVMwareProfile()
        {
            var products = new string[]
            {
                "VMware Virtual Platform",
                "VMware7,1",
                "VMware7,2",
                "VMware8,1",
                "VMware ESXi",
                "VMware Workstation",
                "VMware Fusion"
            };

            string product = products[_random.Next(products.Length)];
            string version;

            if (product.Contains("Platform"))
            {
                version = $"VMware-{_random.Next(1, 10)}.{_random.Next(0, 10)}";
            }
            else if (product.Contains("ESXi"))
            {
                version = $"{_random.Next(6, 8)}.{_random.Next(0, 10)}.{_random.Next(0, 10)}";
            }
            else if (product.Contains("Workstation"))
            {
                version = $"{_random.Next(15, 17)}.{_random.Next(0, 10)}.{_random.Next(0, 10)}";
            }
            else if (product.Contains("Fusion"))
            {
                version = $"{_random.Next(11, 13)}.{_random.Next(0, 10)}.{_random.Next(0, 10)}";
            }
            else
            {
                version = $"{_random.Next(1, 10)}.{_random.Next(0, 10)}";
            }

            return new ProxmoxVMSMBIOS
            {
                Manufacturer = "VMware, Inc.",
                Product = product,
                Version = version,
                Serial = $"VMware-{GenerateRandomString(8, true)}-{GenerateRandomString(4, true)}-{GenerateRandomString(4, true)}-{GenerateRandomString(4, true)}-{GenerateRandomString(12, true)}",
                UUID = GenerateRandomUUID(),
                Family = "Virtual Machine"
            };
        }

        private static ProxmoxVMSMBIOS GetMicrosoftProfile()
        {
            // Decide between Surface or Server
            bool isSurface = _random.Next(2) == 0;

            string product;
            string family;
            string serial;

            if (isSurface)
            {
                var surfaceModels = new string[]
                {
                    "Surface Pro 8",
                    "Surface Pro 7",
                    "Surface Pro X",
                    "Surface Laptop 4",
                    "Surface Laptop Studio",
                    "Surface Book 3",
                    "Surface Go 3",
                    "Surface Studio 2",
                    "Surface Laptop Go",
                    "Surface Duo 2"
                };

                product = surfaceModels[_random.Next(surfaceModels.Length)];
                family = product.Contains("Laptop") ? "Surface Laptop" :
                         product.Contains("Book") ? "Surface Book" :
                         product.Contains("Studio") ? "Surface Studio" :
                         product.Contains("Go") ? "Surface Go" :
                         product.Contains("Duo") ? "Surface Duo" : "Surface Pro";

                // Surface serial format
                serial = $"{GenerateRandomString(12, false)}";
            }
            else
            {
                var serverModels = new string[]
                {
                    "Azure Stack HCI",
                    "Windows Server 2022",
                    "Windows Server 2019",
                    "Windows Server 2016",
                    "SQL Server 2022",
                    "SQL Server 2019"
                };

                product = serverModels[_random.Next(serverModels.Length)];
                family = product.Contains("Azure") ? "Azure Stack" :
                         product.Contains("SQL") ? "SQL Server" : "Windows Server";

                // Microsoft server serial format
                serial = $"MS-{GenerateRandomString(8, false)}";
            }

            return new ProxmoxVMSMBIOS
            {
                Manufacturer = "Microsoft Corporation",
                Product = product,
                Version = $"{_random.Next(1, 10)}.{_random.Next(0, 10)}",
                Serial = serial,
                UUID = GenerateRandomUUID(),
                Family = family
            };
        }

        private static ProxmoxVMSMBIOS GetHyperVProfile()
        {
            var versions = new string[]
            {
                "2016",
                "2019",
                "2022",
                "10",
                "11"
            };

            string version = versions[_random.Next(versions.Length)];
            string product = version == "10" || version == "11" ?
                            $"Windows {version} Hyper-V" :
                            $"Hyper-V Server {version}";

            return new ProxmoxVMSMBIOS
            {
                Manufacturer = "Microsoft Corporation",
                Product = product,
                Version = $"{_random.Next(1, 10)}.{_random.Next(0, 10)}",
                Serial = $"VM{_random.Next(1000000, 9999999)}",
                UUID = GenerateRandomUUID(),
                Family = "Virtual Machine"
            };
        }

        private static ProxmoxVMSMBIOS GetVirtualBoxProfile()
        {
            var versions = new string[]
            {
                "5.2",
                "6.0",
                "6.1",
                "7.0",
                "7.1"
            };

            string version = versions[_random.Next(versions.Length)];
            string buildNumber = $"{_random.Next(1, 50)}";

            return new ProxmoxVMSMBIOS
            {
                Manufacturer = "Oracle Corporation",
                Product = $"VirtualBox {version}",
                Version = $"{version}.{buildNumber}",
                Serial = $"0",  // VirtualBox typically uses "0" as the serial number
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
