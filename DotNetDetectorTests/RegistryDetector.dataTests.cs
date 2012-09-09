using System;
using System.Linq;
using DotNetDetector;
using NUnit.Framework;
using Rhino.Mocks;
using Microsoft.Win32;

namespace DotNetDetectorTests
{
    [TestFixture]
    public class RegistryDetectorDataTests
    {
        [Test]
        public void GetServicePacks_NoMatch_EmptyList()
        {
            // Fixture setup...
            const string keyName = "keyName";

            var detection =
                MockRepository.GeneratePartialMock<RegistryDetection>();
            detection
                .Expect(d => d.FullProfileRegistryKeyName)
                .Return(keyName);

            var key = MockRepository.GenerateMock<RegistryKeyBase>(
                RegistryHive.LocalMachine
            );
            key
                .Expect(k => k.MatchRegistryValue(keyName, "SP", 1))
                .Return(false);
            key
                .Expect(k => k.MatchRegistryValue(keyName, "SP", 2))
                .Return(false);

            // Exercise SUT...
            var packs = RegistryDetector.GetServicePacks(key, detection);

            // Verify SUT...
            detection.VerifyAllExpectations();
            key.VerifyAllExpectations();
            Assert.That(packs, Is.Empty);

            // Fixture teardown by GC...
        }

        [Test]
        public void GetServicePacks_Match_Sp1()
        {
            // Fixture setup...
            const string keyName = "otherKeyName";

            var detection =
                MockRepository.GeneratePartialMock<RegistryDetection>();
            detection
                .Expect(d => d.FullProfileRegistryKeyName)
                .Return(keyName);

            var key = MockRepository.GenerateMock<RegistryKeyBase>(
                RegistryHive.LocalMachine
            );
            key
                .Expect(k => k.MatchRegistryValue(keyName, "SP", 1))
                .Return(true);

            // Exercise SUT...
            var packs = RegistryDetector.GetServicePacks(key, detection);

            // Verify SUT...
            detection.VerifyAllExpectations();
            key.VerifyAllExpectations();
            Assert.That(packs.Count(), Is.EqualTo(1));
            Assert.That(packs.First(), Is.EqualTo(new Version("1.0")));

            // Fixture teardown by GC...
        }

        [Test]
        public void GetServicePacks_Match_Sp2()
        {
            // Fixture setup...
            const string keyName = "otherKeyName";

            var detection =
                MockRepository.GeneratePartialMock<RegistryDetection>();
            detection
                .Expect(d => d.FullProfileRegistryKeyName)
                .Return(keyName);

            var key = MockRepository.GenerateMock<RegistryKeyBase>(
                RegistryHive.LocalMachine
            );
            key
                .Expect(k => k.MatchRegistryValue(keyName, "SP", 1))
                .Return(false);
            key
                .Expect(k => k.MatchRegistryValue(keyName, "SP", 2))
                .Return(true);

            // Exercise SUT...
            var packs = RegistryDetector.GetServicePacks(key, detection);

            // Verify SUT...
            detection.VerifyAllExpectations();
            key.VerifyAllExpectations();
            Assert.That(packs.Count(), Is.EqualTo(2));
            Assert.That(packs.First(), Is.EqualTo(new Version("1.0")));
            Assert.That(packs.Last(), Is.EqualTo(new Version("2.0")));

            // Fixture teardown by GC...
        }

        [Test]
        public void RegistryDetections_Sp2UsesGeneralSpDetector()
        {
            // Fixture setup...
            var dot2Detection = RegistryDetector
                .RegistryDetections
                .Single(d => d.VersionBuilder.Version.Equals(
                    new Version("2.0")
                ));

            // Exercise SUT...
            var dlgt = dot2Detection.GetServicePacksDelegate;

            // Verify SUT...
            Assert.That(dlgt == RegistryDetector.GetServicePacks, Is.True);

            // Fixture teardown by GC...
        }
    }
}
