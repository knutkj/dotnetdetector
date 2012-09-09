using NUnit.Framework;
using Rhino.Mocks;
using DotNetDetector;
using System;
using Microsoft.Win32;

namespace DotNetDetectorTests
{
    [TestFixture]
    public class DetectorTests
    {
        [Test]
        public void CurrentLazyInitsARegDetector()
        {
            // Fixture setup...

            // Exercise SUT...
            var current = Detector.Current;

            // Verify SUT...            
            Assert.That(current, Is.InstanceOf<RegistryDetector>());
            Assert.That(Detector.Current, Is.SameAs(current));
            var registryDetector = (RegistryDetector)current;
            var key = (RegistryKeyWrapper)registryDetector.RootKey;
            Assert.That(key.WrappedKey, Is.SameAs(Registry.LocalMachine));
            Assert.That(key.Hive, Is.EqualTo(RegistryHive.LocalMachine));

            // Fixture teardown...
            Detector.Current = null;
        }

        [Test]
        public void CurrentSavesSpecified()
        {
            // Fixture setup...
            var expectedDetector = MockRepository.GenerateStub<IDetector>();

            // Exercise SUT...
            Detector.Current = expectedDetector;

            // Verify SUT...            
            Assert.That(Detector.Current, Is.EqualTo(expectedDetector));

            // Fixture teardown...
            Detector.Current = null;
        }

        [Test]
        public void VersionsDelegatesToCurrentDetector()
        {
            // Fixture setup...
            var expectedVersions = new DotNetVersion[0];
            var detector = MockRepository.GenerateMock<IDetector>();
            detector.Expect(d => d.Versions).Return(expectedVersions);
            Detector.Current = detector;

            // Exercise SUT...
            var actualVersions = Detector.Versions;

            // Verify SUT...
            detector.VerifyAllExpectations();
            Assert.That(actualVersions, Is.EqualTo(expectedVersions));

            // Fixture teardown...
            Detector.Current = null;
        }

        [Test]
        public void MaxDotNetVersion_NoVersions_Null()
        {
            // Fixture setup...
            var detector = MockRepository.GenerateMock<IDetector>();
            detector.Expect(d => d.Versions).Return(null);
            Detector.Current = detector;

            // Exercise SUT...
            var maxDotNetVersion = Detector.MaxDotNetVersion;

            // Verify SUT...
            Assert.That(maxDotNetVersion, Is.EqualTo(null));

            // Fixture teardown...
            Detector.Current = null;
        }

        [Test]
        public void MaxDotNetVersion_NoVersion_Null()
        {
            // Fixture setup...
            var detector = MockRepository.GenerateMock<IDetector>();
            detector.Expect(d => d.Versions).Return(new DotNetVersion[0]);
            Detector.Current = detector;

            // Exercise SUT...
            var maxDotNetVersion = Detector.MaxDotNetVersion;

            // Verify SUT...
            Assert.That(maxDotNetVersion, Is.EqualTo(null));

            // Fixture teardown...
            Detector.Current = null;
        }

        [Test]
        public void MaxDotNetVersion_OneVersion_ThatOne()
        {
            // Fixture setup...
            var version = new DotNetVersion(new Version("1.0"));
            var detector = MockRepository.GenerateMock<IDetector>();
            detector.Expect(d => d.Versions).Return(new[] { version });
            Detector.Current = detector;

            // Exercise SUT...
            var maxDotNetVersion = Detector.MaxDotNetVersion;

            // Verify SUT...
            Assert.That(maxDotNetVersion, Is.EqualTo(version));

            // Fixture teardown...
            Detector.Current = null;
        }

        [Test]
        public void MaxDotNetVersion_TwoVersions_MaxOne()
        {
            // Fixture setup...
            var version1 = new DotNetVersion(new Version("1.0"));
            var version2 = new DotNetVersion(new Version("2.0"));
            var detector = MockRepository.GenerateMock<IDetector>();
            detector.Expect(d => d.Versions).Return(new[] { 
                version1,
                version2
            });
            Detector.Current = detector;

            // Exercise SUT...
            var maxDotNetVersion = Detector.MaxDotNetVersion;

            // Verify SUT...
            Assert.That(maxDotNetVersion, Is.EqualTo(version2));

            // Fixture teardown...
            Detector.Current = null;
        }
    }
}
