using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;

namespace DotNetDetector
{
    /// <summary>
    /// Represents a Microsoft .NET Framework version detector which
    /// uses the Windows Registry to detect versions. The detector has a list
    /// of default <see cref="RegistryDetectionSpecification"/> that may be
    /// overridden. See the <see cref="RegistryDetectionSpecifications"/>
    /// property.
    /// </summary>
    public class RegistryDetector : IDetector
    {
        /// <summary>
        /// Get or set the list of registry detection specifications.
        /// </summary>
        public static List<RegistryDetectionSpecification> 
            RegistryDetectionSpecifications
        {
            get { return _registryDetectionSpecs; }
            set { _registryDetectionSpecs = value; }
        }

        /// <summary>
        /// Get the detected Microsoft .NET Framework versions.
        /// </summary>
        public IEnumerable<DotNetVersion> Versions
        {
            get
            {
                var detections = new List<Detection>();
                var temporaryVersions = new List<DotNetVersion>();
                foreach (var spec in RegistryDetectionSpecifications)
                {
                    var version = Detect(spec);
                    if (version != null)
                    {
                        detections.Add(new Detection(spec, Detect(spec)));
                        temporaryVersions.Add(version);
                    }
                }
                var finalVersions = new List<DotNetVersion>();
                foreach (var detection in detections)
                {
                    var isValidDetection =
                        detection.Specification
                            .IsValidDetectionDelegate == null ||
                        detection.Specification
                            .IsValidDetectionDelegate(temporaryVersions);
                    if (isValidDetection)
	                {
                        finalVersions.Add(detection.Version);
	                }
                }
                return finalVersions;
            }
        }

        private DotNetVersion Detect(RegistryDetectionSpecification spec)
        {
            var key = Registry.LocalMachine.OpenSubKey(
                spec.RegistryKey,
                false
            );
            using (key)
            {
                object value = null;
                var versionIsInstalled =
                    key != null &&
                    (value = key.GetValue(spec.ValueKey)) != null &&
                    value.Equals(spec.Value);
                if (versionIsInstalled)
                {
                    var version = spec.Version;
                    if (spec.GetServicePacksDelegate != null)
                    {
                        version = new DotNetVersion(
                            version.Version,
                            spec.GetServicePacksDelegate(key),
                            version.Profiles
                        );
                    }
                    return version;
                }
                return null;
            }
        }

        private class Detection
        {
            public Detection(
                RegistryDetectionSpecification specification,
                DotNetVersion version
            )
            {
                Specification = specification;
                Version = version;
            }
            public RegistryDetectionSpecification Specification { get; set; }
            public DotNetVersion Version { get; set; }
        }

        #region Detection data.

        private static List<RegistryDetectionSpecification> _registryDetectionSpecs =
            new List<RegistryDetectionSpecification>
            {
                // Detects .NET 4.0 Full Profile.
                new RegistryDetectionSpecification
                {
                    RegistryKey =
                        @"SOFTWARE\Microsoft\" +
                        @"NET Framework Setup\NDP\v4\Full",
                    ValueKey = "Install",
                    Value = 1,
                    Version = new DotNetVersion(
                        new Version("4.0"),
                        DotNetProfiles.ClientFull
                    )
                },

                // Detects .NET 4.0 Client Profile.
                new RegistryDetectionSpecification
                {
                    RegistryKey =
                        @"SOFTWARE\Microsoft\" +
                        @"NET Framework Setup\NDP\v4\Client",
                    ValueKey = "Install",
                    Value = 1,
                    Version = new DotNetVersion(
                        new Version("4.0"),
                        DotNetProfiles.Client
                    ),
                    IsValidDetectionDelegate =
                        DotNet4ClientProfileIsValidDetection
                },

                // Detects .NET 3.5 with service packs.
                new RegistryDetectionSpecification
                {
                    RegistryKey =
                        @"SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5",
                    ValueKey = "Install",
                    Value = 1,
                    Version = new DotNetVersion(new Version("3.5")),
                    GetServicePacksDelegate = Get35ServicePack
                },

                // Detects .NET 3.0 with service packs.
                new RegistryDetectionSpecification
                {
                    RegistryKey =
                        @"SOFTWARE\Microsoft\" + 
                        @"NET Framework Setup\NDP\v3.0\Setup",
                    ValueKey = "InstallSuccess",
                    Value = 1,
                    Version = new DotNetVersion(new Version("3.0")),
                    GetServicePacksDelegate = Get3ServicePack
                },

                // Detects .NET 2.0 with service packs.
                new RegistryDetectionSpecification
                {
                    RegistryKey =
                        @"Software\Microsoft\" +
                        @"NET Framework Setup\NDP\v2.0.50727",
                    ValueKey = "Install",
                    Value = 1,
                    Version = new DotNetVersion(new Version("2.0")),
                    GetServicePacksDelegate = Get2ServicePack
                }
            };

        /// <summary>
        /// Detects if the .NET 4.0 Full Profil is detected. If so the Client
        /// Profile detection is not valid since the Full Profile contains the
        /// Client profile.
        /// </summary>
        private static bool DotNet4ClientProfileIsValidDetection(
            IEnumerable<DotNetVersion> detectedVersions
        )
        {
            foreach (var version in detectedVersions)
            {
                var full40detected =
                    version.Version == new Version("4.0") &&
                    (version.Profiles & DotNetProfiles.Full) ==
                        DotNetProfiles.Full;
                if (full40detected)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Get the .NET 3.5 service packs.
        /// </summary>
        private static IEnumerable<Version> Get35ServicePack(RegistryKey key)
        {
            return key.GetValue("SP").Equals(1) ?
                new[] { new Version("1.0") } :
                new Version[0];
        }

        /// <summary>
        /// Get the .NET 3.0 service packs.
        /// </summary>
        private static IEnumerable<Version> Get3ServicePack(RegistryKey key)
        {
            var spKeyName = Path
                .GetDirectoryName(key.Name)
                .Replace(Registry.LocalMachine.Name + @"\", "");
            var spKey = Registry.LocalMachine.OpenSubKey(spKeyName, false);
            using (spKey)
            {
                if (spKey == null)
                {
                    return new Version[0];
                }
                var installKeyValue = spKey.GetValue("Install");
                if (installKeyValue != null && installKeyValue.Equals(1))
                {
                    return Get2ServicePack(spKey);
                }
                return new Version[0];
            }
        }

        /// <summary>
        /// Get the .NET 2.0 service packs.
        /// </summary>
        private static IEnumerable<Version> Get2ServicePack(RegistryKey key)
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

        #endregion
    }
}
