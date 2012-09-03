using System.Collections.Generic;

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
                    (_currentDetector = new RegistryDetector());
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
    }
}
