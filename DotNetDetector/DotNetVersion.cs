using System;
using System.Collections.Generic;

namespace DotNetDetector
{
    /// <summary>
    /// Represents a detected Microsoft .NET Framework version.
    /// </summary>
    public class DotNetVersion
    {
        private readonly Version _version;
        private readonly DotNetProfiles _profiles;
        private readonly IEnumerable<Version> _servicePacks;

        /// <summary>
        /// Initializes a new instance of a detected 
        /// <see cref="DotNetVersion"/> of the specified version, with
        /// the specified service packs and of the specified profile types.
        /// </summary>
        /// <remarks>
        /// Not that the <see cref="DotNetProfiles"/> is a flag enumeration
        /// meaning that it can represent more than one type of profile.
        /// </remarks>
        /// <param name="version">
        /// The detected .NET version.
        /// </param>
        /// <param name="servicePacks">
        /// The detected service packs for the .NET version.
        /// </param>
        /// <param name="profiles">
        /// The detected .NET profiles.
        /// </param>
        public DotNetVersion(
            Version version,
            IEnumerable<Version> servicePacks,
            DotNetProfiles profiles
        )
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }
            if (servicePacks == null)
            {
                throw new ArgumentNullException("servicePacks");
            }
            _version = version;
            _servicePacks = servicePacks;
            _profiles = profiles;
        }

        /// <summary>
        /// Initializes a new instance of a detected 
        /// <see cref="DotNetVersion"/> of the specified version, with
        /// the specified service packs and with all profiles.
        /// </summary>
        /// <param name="version">
        /// The detected .NET version.
        /// </param>
        /// <param name="servicePacks">
        /// The profile types.
        /// </param>
        public DotNetVersion(
            Version version,
            IEnumerable<Version> servicePacks
        )
            : this(version, servicePacks, DotNetProfiles.ClientFull)
        { }

        /// <summary>
        /// Initializes a new instance of a detected 
        /// <see cref="DotNetVersion"/> of the specified version, with
        /// no service packs and the specified profiles.
        /// </summary>
        /// <param name="version">
        /// The detected .NET version.
        /// </param>
        /// <param name="profiles">
        /// The detected .NET profiles.
        /// </param>
        public DotNetVersion(Version version, DotNetProfiles profiles)
            : this(version, new Version[0], profiles)
        { }

        /// <summary>
        /// Initializes a new instance of a detected 
        /// <see cref="DotNetVersion"/> of the specified version, with
        /// no service packs and all profiles.
        /// </summary>
        /// <param name="version">
        /// The detected .NET version.
        /// </param>
        public DotNetVersion(Version version) :
            this(version, new Version[0], DotNetProfiles.ClientFull)
        { }

        /// <summary>
        /// Get the detected Microsoft .NET Framework version.
        /// </summary>
        public Version Version
        {
            get { return _version; }
        }

        /// <summary>
        /// Get the detected Microsoft .NET Framework service packs.
        /// </summary>
        public IEnumerable<Version> ServicePacks
        {
            get { return _servicePacks; }
        }

        /// <summary>
        /// Get the detected Microsoft .NET Framework profiles.
        /// </summary>
        public DotNetProfiles Profiles
        {
            get { return _profiles; }
        }

        /// <summary>
        /// Get a <see cref="string"/> that represents this
        /// Microsoft .NET Framework version.
        /// </summary>
        public override string ToString()
        {
            var profile =
                (Profiles & DotNetProfiles.Full) == DotNetProfiles.Full ?
                    " Full Profile" : " Client Profile";
            Version maxSp = null;
            foreach (var csp in ServicePacks)
            {
                if (maxSp == null || maxSp.Major < csp.Major)
                {
                    maxSp = csp;
                }
            }
            var sp = maxSp == null ?
                "" : string.Format(" Service Pack {0}", maxSp.Major);
            return string.Format(
                "Microsoft .NET Framework {0}{1}{2}",
                Version.ToString(),
                sp,
                profile
            );
        }
    }
}