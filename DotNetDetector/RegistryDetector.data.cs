using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;

namespace DotNetDetector
{
    public partial class RegistryDetector
    {
        private static List<RegistryDetection> _registryDetectionSpecs =
            new List<RegistryDetection>
            {
                // Detects .NET 4.0.
                new RegistryDetection
                {
                    VersionBuilder = new DotNetVersionBuilder
                    {
                        Version = new Version("4.0"),
                    },
                    FullProfileRegistryKeyName =
                        @"SOFTWARE\Microsoft\" +
                        @"NET Framework Setup\NDP\v4\Full",
                    FullProfileValueName = "Install",
                    FullProfileValue = 1,
                    ClientProfileRegistryKeyName =
                        @"SOFTWARE\Microsoft\" +
                        @"NET Framework Setup\NDP\v4\Client",
                    ClientProfileValueName = "Install",
                    ClientProfileValue = 1,
                    GetProfilesDelegate = Get40Profiles
                },

                // Detects .NET 3.5 with service packs.
                new RegistryDetection
                {
                    FullProfileRegistryKeyName =
                        @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5",
                    FullProfileValueName = "Install",
                    FullProfileValue = 1,
                    VersionBuilder = new DotNetVersionBuilder
                    {
                        Version = new Version("3.5")
                    },
                    GetServicePacksDelegate = Get35ServicePack
                },

                // Detects .NET 3.0 with service packs.
                new RegistryDetection
                {
                    VersionBuilder = new DotNetVersionBuilder
                    {
                        Version = new Version("3.0")
                    },
                    FullProfileRegistryKeyName =
                        @"SOFTWARE\Microsoft\" + 
                        @"NET Framework Setup\NDP\v3.0\Setup",
                    FullProfileValueName = "InstallSuccess",
                    FullProfileValue = 1,
                    GetServicePacksDelegate = Get3ServicePack
                },

                // Detects .NET 2.0 with service packs.
                new RegistryDetection
                {
                    VersionBuilder = new DotNetVersionBuilder
                    {
                        Version = new Version("2.0")
                    },
                    FullProfileRegistryKeyName =
                        @"Software\Microsoft\" +
                        @"NET Framework Setup\NDP\v2.0.50727",
                    FullProfileValueName = "Install",
                    FullProfileValue = 1,
                    GetServicePacksDelegate = Get2ServicePack
                }
            };

        /// <summary>
        /// Get the .NET 4.0 profiles.
        /// </summary>
        private static DotNetProfiles Get40Profiles(
            RegistryKeyBase key,
            RegistryDetection detection
        )
        {
            if (detection.FullProfileDetected)
            {
                return DotNetProfiles.ClientFull;
            }
            return DotNetProfiles.Client;
        }

        /// <summary>
        /// Get the .NET 3.5 service packs.
        /// </summary>
        private static IEnumerable<Version> Get35ServicePack(
            RegistryKeyBase key,
            RegistryDetection detection
        )
        {
            return key
                .OpenSubKey(detection.FullProfileRegistryKeyName)
                .GetValue("SP").Equals(1) ?
                    new[] { new Version("1.0") } :
                    new Version[0];
        }

        /// <summary>
        /// Get the .NET 3.0 service packs.
        /// </summary>
        private static IEnumerable<Version> Get3ServicePack(
            RegistryKeyBase key,
            RegistryDetection detection
        )
        {
            var spKeyName = Path
                .GetDirectoryName(
                    key.OpenSubKey(detection.FullProfileRegistryKeyName).Name
                )
                .Replace(Registry.LocalMachine.Name + @"\", "");
            var spKey = key.OpenSubKey(spKeyName);
            using (spKey)
            {
                if (spKey == null)
                {
                    return new Version[0];
                }
                var installKeyValue = spKey.GetValue("Install");
                if (installKeyValue != null && installKeyValue.Equals(1))
                {
                    return GetServicePacks(spKey);
                }
                return new Version[0];
            }
        }

        /// <summary>
        /// Get the .NET 2.0 service packs.
        /// </summary>
        private static IEnumerable<Version> Get2ServicePack(
            RegistryKeyBase key,
            RegistryDetection detection
        )
        {
            return GetServicePacks(
                key.OpenSubKey(detection.FullProfileRegistryKeyName)
            );
        }

        /// <summary>
        /// Get the .NET 2.0 service packs.
        /// </summary>
        private static IEnumerable<Version> GetServicePacks(
            RegistryKeyBase key
        )
        {
            var value = key.GetValue("SP");
            if (value.Equals(1))
            {
                return new[] {
                    new Version("1.0") 
                };
            }
            if (value.Equals(2))
            {
                return new[] { 
                    new Version("1.0"),
                    new Version("2.0")
                };
            }
            return new Version[0];
        }
    }
}
