using System;

namespace DotNetDetector
{
    /// <summary>
    /// Microsoft .NET Framework profiles.
    /// </summary>
    [Flags]
    public enum DotNetProfiles
    {
        /// <summary>
        /// Microsoft .NET Framework Client Profile.
        /// </summary>
        Client = 1,

        /// <summary>
        /// Microsoft .NET Framework Full Profile.
        /// </summary>
        Full = 2,

        /// <summary>
        /// Microsoft .NET Framework Client and Full Profile.
        /// </summary>
        ClientFull = Client | Full
    }
}