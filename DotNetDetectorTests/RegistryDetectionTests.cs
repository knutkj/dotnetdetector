using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DotNetDetector;
using Rhino.Mocks;
using Microsoft.Win32;

namespace DotNetDetectorTests
{
    [TestFixture]
    public class RegistryDetectionTests
    {
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void ValidateSpecification_NoKeyName_Exception()
        {
            // Fixture setup...
            var detection = new RegistryDetection();

            // Exercise and verify SUT...
            detection.Validate();

            // Fixture teardown by GC...
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void ValidateSpecification_NoFullProfilValueName_Exception()
        {
            // Fixture setup...
            var detection = new RegistryDetection
            {
                FullProfileRegistryKeyName = "keyName"
            };

            // Exercise and verify SUT...
            detection.Validate();

            // Fixture teardown by GC...
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void ValidateSpecification_NoFullProfilValue_Exception()
        {
            // Fixture setup...
            var detection = new RegistryDetection
            {
                FullProfileRegistryKeyName = "keyName",
                FullProfileValueName = "valueName"
            };

            // Exercise and verify SUT...
            detection.Validate();

            // Fixture teardown by GC...
        }

        [Test]
        public void ValidateSpecification_AllFullProps_Valid()
        {
            // Fixture setup...
            var detection = new RegistryDetection
            {
                FullProfileRegistryKeyName = "keyName",
                FullProfileValueName = "valueName",
                FullProfileValue = "value",
                VersionBuilder = new DotNetVersionBuilder()
            };

            // Exercise and verify SUT...
            detection.Validate();

            // Fixture teardown by GC...
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void ValidateSpecification_NoClientProfilValueName_Exception()
        {
            // Fixture setup...
            var detection = new RegistryDetection
            {
                ClientProfileRegistryKeyName = "keyName"
            };

            // Exercise and verify SUT...
            detection.Validate();

            // Fixture teardown by GC...
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void ValidateSpecification_NoClientProfilValue_Exception()
        {
            // Fixture setup...
            var detection = new RegistryDetection
            {
                ClientProfileRegistryKeyName = "keyName",
                ClientProfileValueName = "valueName"
            };

            // Exercise and verify SUT...
            detection.Validate();

            // Fixture teardown by GC...
        }

        [Test]
        public void ValidateSpecification_AllClientProps_Valid()
        {
            // Fixture setup...
            var detection = new RegistryDetection
            {
                ClientProfileRegistryKeyName = "keyName",
                ClientProfileValueName = "valueName",
                ClientProfileValue = "value",
                VersionBuilder = new DotNetVersionBuilder()
            };

            // Exercise and verify SUT...
            detection.Validate();

            // Fixture teardown by GC...
        }

        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void ValidateSpecification_NoBuilder_Exception()
        {
            // Fixture setup...
            var detection = new RegistryDetection
            {
                ClientProfileRegistryKeyName = "keyName",
                ClientProfileValueName = "valueName",
                ClientProfileValue = "value"
            };

            // Exercise and verify SUT...
            detection.Validate();

            // Fixture teardown by GC...
        }

        [Test]
        public void Detect_NoKeysHack_SilentAndNull()
        {
            // Fixture setup...
            var spec = MockRepository
                .GeneratePartialMock<RegistryDetection>();
            spec.Expect(s => s.Validate());

            var key = MockRepository.GenerateMock<RegistryKeyBase>(
                RegistryHive.DynData
            );

            // Exercise SUT...
            var res = spec.Detect(key);

            // Verify SUT...
            key.AssertWasNotCalled(
                k => k.MatchRegistryValue(null, null, null),
                o => o.IgnoreArguments()
            );
            spec.AssertWasNotCalled(s => s.GetServicePacksDelegate);
            spec.AssertWasNotCalled(s => s.GetProfilesDelegate);
            spec.VerifyAllExpectations();
            Assert.That(res, Is.Null);

            // Fixture teardown by GC...
        }

        [Test]
        public void Detect_FullProfileNotMatch_Null()
        {
            // Fixture setup...
            const string keyName = "subKey";
            const string valueName = "valueName";
            var value = 1;
            const bool fullProfileDetected = false;

            var spec = MockRepository
                .GeneratePartialMock<RegistryDetection>();
            spec.Expect(s => s.Validate());
            spec.Expect(s => s.FullProfileRegistryKeyName).Return(keyName);
            spec.Expect(s => s.FullProfileValueName).Return(valueName);
            spec.Expect(s => s.FullProfileValue).Return(value);
            spec.Expect(s => s.FullProfileDetected = fullProfileDetected);

            var key = MockRepository.GenerateMock<RegistryKeyBase>(
                RegistryHive.LocalMachine
            );
            key
                .Expect(d => d.MatchRegistryValue(keyName, valueName, value))
                .Return(fullProfileDetected);

            // Exercise SUT...
            var res = spec.Detect(key);

            // Verify SUT...
            spec.VerifyAllExpectations();
            key.VerifyAllExpectations();
            Assert.That(res, Is.Null);

            // Fixture teardown by GC...
        }

        [Test]
        public void Detect_FullProfileMatch_Version()
        {
            // Fixture setup...
            const string keyName = "subKey";
            const string valueName = "valueName";
            var value = 1;
            const bool fullProfileDetected = true;

            var expectedVersion = new DotNetVersion(new Version("1.0"));
            var builder = MockRepository
                .GeneratePartialMock<DotNetVersionBuilder>();
            builder.Expect(b => b.DotNetVersion).Return(expectedVersion);

            var spec = MockRepository
                .GeneratePartialMock<RegistryDetection>();
            spec.Expect(s => s.Validate());
            spec.Expect(s => s.FullProfileRegistryKeyName).Return(keyName);
            spec.Expect(s => s.FullProfileValueName).Return(valueName);
            spec.Expect(s => s.FullProfileValue).Return(value);
            spec.Expect(s => s.FullProfileDetected = fullProfileDetected);
            spec.Expect(s => s.ClientProfileDetected = true);
            spec.Expect(s => s.VersionBuilder).Return(builder);

            var key = MockRepository.GenerateMock<RegistryKeyBase>(
                RegistryHive.PerformanceData
            );
            key
                .Expect(d => d.MatchRegistryValue(keyName, valueName, value))
                .Return(fullProfileDetected);

            // Exercise SUT...
            var actualVersion = spec.Detect(key);

            // Verify SUT...
            spec.VerifyAllExpectations();
            key.VerifyAllExpectations();
            Assert.That(actualVersion, Is.SameAs(expectedVersion));

            // Fixture teardown by GC...
        }

        [Test]
        public void Detect_ClientProfileNotMatch_Null()
        {
            // Fixture setup...
            const string keyName = "subKey";
            const string valueName = "valueName";
            var value = 1;
            const bool clientProfileDetected = false;

            var spec = MockRepository
                .GeneratePartialMock<RegistryDetection>();
            spec.Expect(s => s.Validate());
            spec.Expect(s => s.FullProfileRegistryKeyName).Return(null);
            spec.Expect(s => s.ClientProfileRegistryKeyName).Return(keyName);
            spec.Expect(s => s.ClientProfileValueName).Return(valueName);
            spec.Expect(s => s.ClientProfileValue).Return(value);
            spec.Expect(s => s.ClientProfileDetected = clientProfileDetected);

            var key = MockRepository.GenerateMock<RegistryKeyBase>(
                RegistryHive.Users
            );
            key
                .Expect(d => d.MatchRegistryValue(keyName, valueName, value))
                .Return(clientProfileDetected);

            // Exercise SUT...
            var res = spec.Detect(key);

            // Verify SUT...
            spec.VerifyAllExpectations();
            key.VerifyAllExpectations();
            Assert.That(res, Is.Null);

            // Fixture teardown by GC...
        }

        [Test]
        public void Detect_GetServicePakcsAndProfilesDelegate_Overrides()
        {
            // Fixture setup...
            var clientRegKeyName = "clientRegKeyName";
            var expectedServicePacks = new Version[0];
            var expectedProfiles = DotNetProfiles.Client;
            var version = new DotNetVersion(new Version("1.0"));

            var builder = MockRepository
                .GeneratePartialMock<DotNetVersionBuilder>();
            builder.Expect(b => b.ServicePacks = expectedServicePacks);
            builder.Expect(b => b.Profiles = expectedProfiles);
            builder.Expect(b => b.DotNetVersion).Return(version);

            var key = MockRepository.GeneratePartialMock<RegistryKeyBase>(
                RegistryHive.LocalMachine
            );
            key
                .Expect(k => k.MatchRegistryValue(
                    clientRegKeyName,
                    null,
                    null
                ))
                .Return(true);

            var detection = MockRepository
                .GeneratePartialMock<RegistryDetection>();
            detection.ClientProfileRegistryKeyName = clientRegKeyName;
            detection.VersionBuilder = builder;
            detection.Expect(d => d.Validate());
            detection
                .Expect(d => d.GetServicePacksDelegate)
                .Return((k, d) =>
                {
                    Assert.That(k, Is.SameAs(key));
                    Assert.That(d, Is.SameAs(detection));
                    return expectedServicePacks;
                })
                .Repeat
                .Twice();
            detection
                .Expect(d => d.GetProfilesDelegate)
                .Return((k, d) =>
                {
                    Assert.That(k, Is.SameAs(key));
                    Assert.That(d, Is.SameAs(detection));
                    return expectedProfiles;
                })
                .Repeat
                .Twice();

            // Exercise SUT...
            var actualVersion = detection.Detect(key);

            // Verify SUT...
            detection.VerifyAllExpectations();
            builder.VerifyAllExpectations();
            Assert.That(actualVersion, Is.EqualTo(version));

            // Fixture teardown by GC...
        }
    }
}
