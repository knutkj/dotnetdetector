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
    /// <param name="registryDetection">
    /// The registry detection to base service pack detection upon.
    /// </param>
    /// <returns>
    /// Detected service packs.
    /// </returns>
    public delegate IEnumerable<Version> GetServicePacks(
        RegistryKeyBase key,
        RegistryDetection registryDetection
    );

    /// <summary>
    /// A delegate that detects Microsoft .NET Framework profiles.
    /// </summary>
    /// <param name="key">
    /// The registry key to use as source for profiles detection.
    /// </param>
    /// <param name="registryDetection">
    /// The registry detection to base profile detection upon.
    /// </param>
    /// <returns>
    /// Detected profiles.
    /// </returns>
    public delegate DotNetProfiles GetProfiles(
        RegistryKeyBase key,
        RegistryDetection registryDetection
    );

    /// <summary>
    /// A general specification that satisfies all information needed by the
    /// <see cref="RegistryDetector"/> to perform a Microsoft .NET Framework
    /// version detection.
    /// </summary>
    public class RegistryDetection
    {
        /// <summary>
        /// Get or set the name of the registry key that has values to match
        /// against for the client profile version of the framework. It is
        /// optional to specify this property, but one of the key name
        /// properties must be specified.
        /// </summary>
        public virtual string ClientProfileRegistryKeyName { get; set; }

        /// <summary>
        /// Get or set the name of the registry key value that has the value
        /// to match against for the client profile version of the framework. 
        /// The value is specified in property <see cref="ClientProfileValue"/>.
        /// It is required to specify this property if property
        /// <see cref="ClientProfileRegistryKeyName"/> is specified.
        /// </summary>
        public virtual string ClientProfileValueName { get; set; }

        /// <summary>
        /// Get or set the the registry key value for the client profile
        /// version of the framework that indicates that the version is
        /// installed. If there is a match against this value the detection
        /// is considered successful. It is required to specify this property
        /// if property <see cref="ClientProfileRegistryKeyName"/> is
        /// specified.
        /// </summary>
        public virtual object ClientProfileValue { get; set; }

        /// <summary>
        /// Get or set a value indicating if the client profile was detected.
        /// </summary>
        internal virtual bool ClientProfileDetected { get; set; }

        /// <summary>
        /// Get or set the name of the registry key that has values to match
        /// against for the full profile version of the framework. It is
        /// optional to specify this property, but one of the key name
        /// properties must be specified.
        /// </summary>
        public virtual string FullProfileRegistryKeyName { get; set; }

        /// <summary>
        /// Get or set the name of the registry key value that has the value
        /// to match against for the full profile version of the framework. 
        /// The value is specified in property <see cref="FullProfileValue"/>.
        /// It is required to specify this property if property
        /// <see cref="FullProfileRegistryKeyName"/> is specified.
        /// </summary>
        public virtual string FullProfileValueName { get; set; }

        /// <summary>
        /// Get or set the the registry key value for the full profile version
        /// of the framework that indicates that the version is installed.
        /// If there is a match against this value the detection
        /// is considered successful. It is required to specify this property
        /// if property <see cref="FullProfileRegistryKeyName"/> is
        /// specified.
        /// </summary>
        public virtual object FullProfileValue { get; set; }

        /// <summary>
        /// Get or set a value indicating if the full profile was detected.
        /// </summary>
        internal virtual bool FullProfileDetected { get; set; }

        /// <summary>
        /// Get or set the basis of the Microsoft .NET Framework version that
        /// this detection specification is provided for.
        /// </summary>
        public virtual DotNetVersionBuilder VersionBuilder { get; set; }

        /// <summary>
        /// Get or set a delegate that overrides the service packs specified
        /// in property <see cref="VersionBuilder"/>.
        /// The delegate is optional to specify.
        /// </summary>
        public virtual GetServicePacks GetServicePacksDelegate { get; set; }

        /// <summary>
        /// Get or set a delegate that overrides the profiles specified
        /// in property <see cref="VersionBuilder"/>.
        /// The delegate is optional to specify.
        /// </summary>
        public virtual GetProfiles GetProfilesDelegate { get; set; }

        /// <summary>
        /// Detects if the Microsoft .NET Framework version represented by this
        /// detection is installed.
        /// </summary>
        /// <param name="rootKey">
        /// The root <see cref="RegistryKeyBase"/>.
        /// </param>
        /// <returns>
        /// The detected .NET version or <c>null</c> if the version was
        /// not detected.
        /// </returns>
        public DotNetVersion Detect(RegistryKeyBase rootKey)
        {
            Validate();
            var fullProfileRegistryKeyName = FullProfileRegistryKeyName;
            var clientProfileRegistryKeyName = ClientProfileRegistryKeyName;
            var fullProfileDetected =
                FullProfileDetected = fullProfileRegistryKeyName != null && 
                rootKey.MatchRegistryValue(
                    fullProfileRegistryKeyName,
                    FullProfileValueName,
                    FullProfileValue
                );
            var clientProfileDetected = ClientProfileDetected =
                fullProfileDetected || (
                    clientProfileRegistryKeyName != null &&
                    rootKey.MatchRegistryValue(
                        clientProfileRegistryKeyName,
                        ClientProfileValueName,
                        ClientProfileValue
                    )
                );
            if (!fullProfileDetected && !clientProfileDetected)
            {
                return null;
            }
            if (GetServicePacksDelegate != null)
            {
                VersionBuilder.ServicePacks =
                    GetServicePacksDelegate(rootKey, this);
            }
            if (GetProfilesDelegate != null)
            {
                VersionBuilder.Profiles = GetProfilesDelegate(rootKey, this);
            }
            return VersionBuilder.DotNetVersion;
        }

        /// <summary>
        /// Validates the specificed specification.
        /// </summary>
        internal virtual void Validate()
        {
            if (ClientProfileRegistryKeyName != null)
            {
                if (ClientProfileValueName == null)
                {
                    throw new InvalidOperationException(
                        "ClientProfileValueName property must be specified."
                    );
                }
                if (ClientProfileValue == null)
                {
                    throw new InvalidOperationException(
                        "ClientProfileValue property must be specified."
                    );
                }
            }
            if (FullProfileRegistryKeyName != null)
            {
                if (FullProfileValueName == null)
                {
                    throw new InvalidOperationException(
                        "Registry detection specification must have the " +
                        "FullProfileValueName property specified."
                    );
                }
                if (FullProfileValue == null)
                {
                    throw new InvalidOperationException(
                        "FullProfileValue property must be specified."
                    );
                }
            }
            var noKeyNameSpecified =
                ClientProfileRegistryKeyName == null &&
                FullProfileRegistryKeyName == null;
            if (noKeyNameSpecified)
            {
                throw new InvalidOperationException(
                    "ClientProfileRegistryKeyName or " +
                    "FullProfileRegistryKeyName property must be specified."
                );
            }
            if (VersionBuilder == null)
            {
                throw new InvalidOperationException(
                    "VersionBuilder property must be specified."
                );
            }
        }
    }
}
