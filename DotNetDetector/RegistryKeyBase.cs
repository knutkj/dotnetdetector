using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace DotNetDetector
{
    /// <summary>
    /// Represents a key-level node in the Windows registry.
    /// This interface is a registry encapsulation.
    /// </summary>
    public abstract class RegistryKeyBase : IDisposable
    {
        private readonly RegistryHive _hive;

        internal RegistryKeyBase(RegistryHive hive)
        {
            _hive = hive;
        }

        /// <summary>
        /// Get the key's registry hive.
        /// </summary>
        public RegistryHive Hive
        {
            get { return _hive; }
        }

        /// <summary>
        /// Get the name of the parent key.
        /// </summary>
        /// <param name="hive">
        /// The registry hive.
        /// </param>
        /// <param name="keyName">
        /// The child key name.
        /// </param>
        /// <returns>
        /// The parent key name.
        /// </returns>
        public static string GetParentName(RegistryHive hive, string keyName)
        {
            string rootKeyName = null;
            switch (hive)
            {
                case RegistryHive.LocalMachine:
                    rootKeyName = Registry.LocalMachine.Name;
                    break;
                default:
                    throw new NotImplementedException();
            }
            var parentWithRoot = Path.GetDirectoryName(keyName);
            return Regex.Replace(
                parentWithRoot, 
                "^" + rootKeyName + @"\?", 
                ""
            );
        }

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
