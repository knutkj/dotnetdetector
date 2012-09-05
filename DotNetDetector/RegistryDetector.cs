using System.Collections.Generic;
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
    public partial class RegistryDetector : IDetector
    {
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
    }
}
