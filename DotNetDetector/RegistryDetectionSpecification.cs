using System;
using System.Collections.Generic;
using Microsoft.Win32;

namespace DotNetDetector
{
    /// <summary>
    /// A delegate that detects Microsoft .NET Framework service packs.
    /// </summary>
    /// <param name="key">
    /// The registry key to use as source for service pack detection.
    /// </param>
    /// <returns>
    /// Detected service packs.
    /// </returns>
    public delegate IEnumerable<Version> GetServicePacks(RegistryKey key);

    /// <summary>
    /// A delegate that returns <c>true</c> if to discard the detected
    /// Microsoft .NET Framework version.
    /// </summary>
    /// <param name="detectedVersions">
    /// A list of detected Microsoft .NET Framework versions.
    /// </param>
    /// <returns>
    /// <c>true</c> if to discard the detected .NET Framework version or
    /// <c>false</c>.
    /// </returns>
    public delegate bool
        IsValidDetection(IEnumerable<DotNetVersion> detectedVersions);

    /// <summary>
    /// A general specification that satisfies all information needed by the
    /// <see cref="RegistryDetector"/> to perform a Microsoft .NET Framework
    /// version detection.
    /// </summary>
    public class RegistryDetectionSpecification
    {
        /// <summary>
        /// Get or set the name of the registry key that has values to match
        /// against.
        /// </summary>
        public string RegistryKey { get; set; }

        /// <summary>
        /// Get or set the name of the registry key value that has the value
        /// to match against. The value is specified in property
        /// <see cref="Value"/>.
        /// </summary>
        public string ValueKey { get; set; }

        /// <summary>
        /// Get or set the the registry value of the value specified by
        /// <see cref="ValueKey"/> that needs to be matched. If there is a
        /// match against this value the detection is considered successful.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Get or set the Microsoft .NET Framework version that this detection
        /// specification is provided for.
        /// </summary>
        public DotNetVersion Version { get; set; }

        /// <summary>
        /// Get or set a delegate that overrides the service packs specified
        /// in property <see cref="Version"/>. The delegate is optional to
        /// specify.
        /// </summary>
        public GetServicePacks GetServicePacksDelegate { get; set; }

        /// <summary>
        /// This delegate is executed after all detecions by the
        /// <see cref="RegistryDetector"/> has been executed. If the
        /// delegate returns <c>false</c> the registration is discarded from
        /// the list of detected versions. This property of the specification
        /// is optional.
        /// </summary>
        public IsValidDetection IsValidDetectionDelegate { get; set; }
    }
}
