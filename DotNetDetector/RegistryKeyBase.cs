using System;

namespace DotNetDetector
{
    /// <summary>
    /// Represents a key-level node in the Windows registry.
    /// This interface is a registry encapsulation.
    /// </summary>
    public abstract class RegistryKeyBase : IDisposable
    {
        /// <summary>
        /// Get the name of the key.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Retrieves a specified subkey.
        /// </summary>
        /// <param name="name">
        /// Name or path of the subkey to open.
        /// </param>
        /// <returns>
        /// The subkey requested,
        /// or a <c>null</c> reference if the operation failed.
        /// </returns>
        public abstract RegistryKeyBase OpenSubKey(string name);

        /// <summary>
        /// Retrieves the value associated with the specified name.
        /// Returns a <c>null</c> reference if the name/value pair
        /// does not exist in the registry.
        /// </summary>
        /// <param name="name">
        /// The name of the value to retrieve. 
        /// </param>
        /// <returns>
        /// The value associated with name, or a <c>null</c> reference
        /// if name is not found.
        /// </returns>
        public abstract Object GetValue(string name);

        /// <summary>
        /// Matches a registry value.
        /// </summary>
        public virtual bool MatchRegistryValue(
            string keyName,
            string valueName,
            object value
        )
        {
            using (var key = OpenSubKey(keyName))
            {
                object detectedValue = null;
                return
                    key != null &&
                    (detectedValue = key.GetValue(valueName)) != null &&
                    detectedValue.Equals(value);
            }
        }

        /// <summary>
        /// Disposes the key.
        /// </summary>
        public abstract void Dispose();
    }
}
