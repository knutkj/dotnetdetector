using System;
using Microsoft.Win32;

namespace DotNetDetector
{
    /// <summary>
    /// Represents a key-level node in the Windows registry.
    /// This class is a registry encapsulation.
    /// </summary>
    public class RegistryKeyWrapper : RegistryKeyBase
    {
        private readonly RegistryKey _wrappedKey;

        /// <summary>
        /// Initializes a new instance of <see cref="RegistryKeyWrapper"/>
        /// wrapping the specified <see cref="RegistryKey"/>.
        /// </summary>
        /// <param name="wrappedKey">
        /// The original registry key to wrap.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <c>wrappedKey</c> is <c>null</c>.
        /// </exception>
        public RegistryKeyWrapper(RegistryHive hive, RegistryKey wrappedKey)
            : base(hive)
        {
            if (wrappedKey == null)
            {
                throw new ArgumentNullException("wrappedKey");
            }
            _wrappedKey = wrappedKey;
        }

        /// <summary>
        /// Get the wrapped <see cref="RegistryKey"/>.
        /// </summary>
        internal RegistryKey WrappedKey { get { return _wrappedKey; } }

        /// <summary>
        /// Get the name of the key.
        /// </summary>
        public override string Name { get { return WrappedKey.Name; } }

        /// <summary>
        /// Retrieves a readonly version of the specified subkey.
        /// </summary>
        /// <param name="name">
        /// Name or path of the subkey to open.
        /// </param>
        /// <returns>
        /// The subkey requested,
        /// or a <c>null</c> reference if the operation failed.
        /// </returns>
        public override RegistryKeyBase OpenSubKey(string name)
        {
            var subKey = WrappedKey.OpenSubKey(name, false);
            if (subKey == null)
            {
                return null;
            }
            return new RegistryKeyWrapper(Hive, subKey);
        }

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
        public override object GetValue(string name)
        {
            return WrappedKey.GetValue(name);
        }

        /// <summary>
        /// Disposes the wrapped <see cref="RegistryKey"/>.
        /// </summary>
        public override void Dispose()
        {
            ((IDisposable)WrappedKey).Dispose();
        }
    }
}
