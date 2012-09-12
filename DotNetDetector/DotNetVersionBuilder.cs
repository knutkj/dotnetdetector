using System;
using System.Collections.Generic;

namespace DotNetDetector
{
    /// <summary>
    /// You may use this class to build a new <see cref="DotNetVersion"/>.
    /// </summary>
    /// <remarks>
    /// If you need to JSON deserialize a <see cref="DotNetVersion"/> you may
    /// deserialize into instance of this class and then fetch
    /// <see cref="DotNetVersion"/> from <see cref="DotNetVersion"/> property.
    /// </remarks>
    public class DotNetVersionBuilder
    {
        /// <summary>
        /// Intializes a new instance of <see cref="DotNetVersionBuilder"/>
        /// with the .NET profile set to
        /// <see cref="DotNetProfiles.ClientFull"/>.
        /// </summary>
        public DotNetVersionBuilder()
        {
            Profiles = DotNetProfiles.ClientFull;
        }

        /// <summary>
        /// Get the built <see cref="DotNetVersion"/>.
        /// </summary>
        public virtual DotNetVersion DotNetVersion
        {
            get
            {
                if (Version == null)
                {
                    throw new InvalidOperationException(
                        "You must specify a value for the Version property."
                    );
                }
                if (ServicePacks != null)
                {
                    if (Profiles != DotNetProfiles.ClientFull)
                    {
                        return new DotNetVersion(
                            Version,
                            ServicePacks,
                            Profiles
                        );
                    }
                    return new DotNetVersion(Version, ServicePacks);
                }
                if (Profiles != DotNetProfiles.ClientFull)
                {
                    return new DotNetVersion(Version, Profiles);
                }
                return new DotNetVersion(Version);
            }
        }

        /// <summary>
        /// Get or set the Microsoft .NET Framework version.
        /// </summary>
        public virtual Version Version { get; set; }

        /// <summary>
        /// Get or set the Microsoft .NET Framework service packs.
        /// </summary>
        public virtual IEnumerable<Version> ServicePacks { get; set; }

        /// <summary>
        /// Get or set the Microsoft .NET Framework profiles.
        /// </summary>
        public virtual DotNetProfiles Profiles { get; set; }
    }
}
