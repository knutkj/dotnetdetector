using Microsoft.Win32;
using NUnit.Framework;
using System;
using DotNetDetector;
using Rhino.Mocks;

namespace DotNetDetectorTests
{
    [TestFixture]
    public class RegistryDetectorTests
    {
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NoRootKey_Exception()
        {
            // Fixture setup...

            // Exercise and verify SUT...
            new RegistryDetector(null);

            // Fixture teardown by GC...
        }

        [Test]
        public void Ctor_KeepsRootKeyRef()
        {
            // Fixture setup...
            var key = MockRepository.GenerateStub<RegistryKeyBase>();

            // Exercise SUT...
            var detector = new RegistryDetector(key);

            // Verify SUT...
            Assert.That(detector.RootKey, Is.SameAs(key));

            // Fixture teardown by GC..
        }
    }
}
