using System.Collections.Generic;
using System;
using Microsoft.Win32;

namespace DotNetDetector
{
    /// <summary>
    /// Wraps the current <see cref="IDetector"/>.
    /// </summary>
    public static class Detector
    {
        private static IDetector _currentDetector;

        /// <summary>
        /// Get or set the current Microsoft .NET Framework version detector.
        /// </summary>
        public static IDetector Current
        {
            get
            {
                return _currentDetector ??
                    (_currentDetector = new RegistryDetector(
                        new RegistryKeyWrapper(
                            RegistryHive.LocalMachine,
                            Registry.LocalMachine
                        ))
                    );
            }
            set
            {
                _currentDetector = value;
            }
        }

        /// <summary>
        /// Get the detected Microsoft .NET Framework versions.
        /// </summary>
        public static IEnumerable<DotNetVersion> Versions
        {
            get { return Current.Versions; }
        }

        /// <summary>
        /// Get the maximum detected Microsoft .NET Framework version.
        /// </summary>
        public static DotNetVersion MaxDotNetVersion
        {
            get
            {
                if (Versions == null)
                {
                    return null;
                }
                DotNetVersion maxVersion = null;
                foreach (var version in Versions)
                {
                    if (maxVersion == null)
                    {
                        maxVersion = version;
                    }
                    else if (version.Version > maxVersion.Version)
                    {
                        maxVersion = version;
                    }
                }
                return maxVersion;
            }
        }
    }
}
