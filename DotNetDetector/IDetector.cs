using System.Collections.Generic;

namespace DotNetDetector
{
    /// <summary>
    /// Represents a Microsoft .NET Framework version detector.
    /// </summary>
    public interface IDetector
    {
        /// <summary>
        /// Get the detected Microsoft .NET Framework versions.
        /// </summary>
        IEnumerable<DotNetVersion> Versions { get; }
    }
}
