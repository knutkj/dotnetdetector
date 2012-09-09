using System;
using System.Collections.Generic;

namespace DotNetDetector
{
    public partial class RegistryDetector
    {
        /// <summary>
        /// Get or set the list of registry detections.
        /// </summary>
        public static List<RegistryDetection> RegistryDetections
        {
            get { return _registryDetections; }
            set { _registryDetections = value; }
        }

        private static List<RegistryDetection> _registryDetections =
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
                    GetServicePacksDelegate = GetServicePacks
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
                    GetServicePacksDelegate = GetServicePacks
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
            return detection.FullProfileDetected ?
                DotNetProfiles.ClientFull :
                DotNetProfiles.Client;
        }

        /// <summary>
        /// Get the .NET service packs.
        /// </summary>
        internal static IEnumerable<Version> GetServicePacks(
            RegistryKeyBase key,
            RegistryDetection detection
        )
        {
            var keyName = detection.FullProfileRegistryKeyName;
            return GetServicePacks(key, keyName, "SP");
        }

        /// <summary>
        /// Get the .NET service packs.
        /// </summary>
        internal static IEnumerable<Version> GetServicePacks(
            RegistryKeyBase key,
            string subKeyName,
            string valueName
        )
        {
            return key.MatchRegistryValue(subKeyName, valueName, 1) ?
                new[] { new Version("1.0") } :
                key.MatchRegistryValue(subKeyName, valueName, 2) ?
                    new[] { new Version("1.0"), new Version("2.0") } :
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
            var parentKeyName = RegistryKeyBase.GetParentName(
                key.Hive,
                detection.FullProfileRegistryKeyName
            );
            if (!key.MatchRegistryValue(parentKeyName, "Install", 1))
            {
                return new Version[0];
            }
            return GetServicePacks(key, parentKeyName, "SP");
        }
    }
}
