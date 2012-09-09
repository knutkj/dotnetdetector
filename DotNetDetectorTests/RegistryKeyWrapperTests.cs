using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DotNetDetector;
using Microsoft.Win32;

namespace DotNetDetectorTests
{
    [TestFixture]
    public class RegistryKeyWrapperTests
    {
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NoKey_Exception()
        {
            // Fixture setup...

            // Exercise and verify SUT...
            new RegistryKeyWrapper(RegistryHive.LocalMachine, null);

            // Fixture terdown by GC...
        }

        [Test]
        public void Ctor_KeepsKeyReference()
        {
            // Fixture setup...
            var hive = RegistryHive.LocalMachine;
            var key = Registry.LocalMachine;

            // Exercise SUT...
            var wrapper = new RegistryKeyWrapper(hive, key);

            // Verify SUT...
            Assert.That(wrapper.Hive, Is.EqualTo(hive));
            Assert.That(wrapper.WrappedKey, Is.SameAs(key));

            // Fixture terdown by GC...
        }
    }
}
