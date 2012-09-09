using System.Collections.Generic;
using Microsoft.Win32;
using System;

namespace DotNetDetector
{
    /// <summary>
    /// Represents a Microsoft .NET Framework version detector which
    /// uses the Windows Registry to detect versions. The detector has a list
    /// of default <see cref="RegistryDetection"/> that may be
    /// overridden. See the <see cref="RegistryDetections"/>
    /// property.
    /// </summary>
    public partial class RegistryDetector : IDetector
    {
        private readonly RegistryKeyBase _rootKey;

        /// <summary>
        /// Intializes a new instance of the <see cref="RegistryDetector"/>
        /// with the specified root <see cref="RegistryKeyBase"/>.
        /// </summary>
        /// <param name="rootKey">
        /// The root key.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <c>rootKey</c> is <c>null</c>.
        /// </exception>
        public RegistryDetector(RegistryKeyBase rootKey)
        {
            if (rootKey == null)
            {
                throw new ArgumentNullException("rootKey");
            }
            _rootKey = rootKey;
        }

        /// <summary>
        /// Get the root key.
        /// </summary>
        internal RegistryKeyBase RootKey { get { return _rootKey; } }

        /// <summary>
        /// Get the detected Microsoft .NET Framework versions.
        /// </summary>
        public IEnumerable<DotNetVersion> Versions
        {
            get
            {
                var versions = new List<DotNetVersion>();
                foreach (var spec in RegistryDetections)
                {
                    var version = spec.Detect(RootKey);
                    if (version != null)
                    {
                        versions.Add(version);
                    }
                }
                return versions;
            }
        }
    }
}
