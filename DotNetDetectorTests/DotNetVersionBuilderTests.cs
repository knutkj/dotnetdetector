using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using DotNetDetector;

namespace DotNetDetectorTests
{
    [TestFixture]
    public class DotNetVersionBuilderTests
    {
        [Test, ExpectedException(typeof(InvalidOperationException))]
        public void DotNetVersion_VersionNull_InvalidOperationException()
        {
            // Fixture setup...
            var builder = new DotNetVersionBuilder();

            // Exercise and verify SUT...
            var version = builder.DotNetVersion;

            // Fixture teardown by GC...
        }

        [Test]
        public void DotNetVersion_Version_Built()
        {
            // Fixture setup...
            var version = new Version("1.0");
            var dotNetVersion = new DotNetVersion(version);
            var builder = new DotNetVersionBuilder
            {
                Version = version
            };

            // Exercise SUT...
            var builtVersion = builder.DotNetVersion;

            // Verify SUT...
            Assert.That(builtVersion.Version, Is.SameAs(version));
            Assert.That(builtVersion.ServicePacks, Is.EqualTo(dotNetVersion.ServicePacks));

            // Fixture teardown by GC...
        }

        [Test]
        public void DotNetVersion_VersionAndServicePacks_Built()
        {
            // Fixture setup...
            var version = new Version("2.0");
            var servicePacks = new[] { new Version("1.0") };
            var dotNetVersion = new DotNetVersion(version);
            var builder = new DotNetVersionBuilder
            {
                Version = version,
                ServicePacks = servicePacks
            };

            // Exercise SUT...
            var builtVersion = builder.DotNetVersion;

            // Verify SUT...
            Assert.That(builtVersion.Version, Is.SameAs(version));
            Assert.That(builtVersion.ServicePacks, Is.SameAs(servicePacks));

            // Fixture teardown by GC...
        }

        [Test]
        public void DotNetVersion_VersionAndProfiles_Built()
        {
            // Fixture setup...
            var version = new Version("3.0");
            var profiles = DotNetProfiles.Client;
            var dotNetVersion = new DotNetVersion(version);
            var builder = new DotNetVersionBuilder
            {
                Version = version,
                Profiles = profiles
            };

            // Exercise SUT...
            var builtVersion = builder.DotNetVersion;

            // Verify SUT...
            Assert.That(builtVersion.Version, Is.SameAs(version));
            Assert.That(
                builtVersion.ServicePacks,
                Is.EqualTo(dotNetVersion.ServicePacks)
            );
            Assert.That(builtVersion.Profiles, Is.EqualTo(profiles));

            // Fixture teardown by GC...
        }

        [Test]
        public void DotNetVersion_VersionSpsAndProfiles_Built()
        {
            // Fixture setup...
            var version = new Version("4.0");
            var servicePacks = new[] { 
                new Version("1.0"),
                new Version("2.0")
            };
            var profiles = DotNetProfiles.Full;
            var dotNetVersion = new DotNetVersion(version);
            var builder = new DotNetVersionBuilder
            {
                Version = version,
                ServicePacks = servicePacks,
                Profiles = profiles
            };

            // Exercise SUT...
            var builtVersion = builder.DotNetVersion;

            // Verify SUT...
            Assert.That(builtVersion.Version, Is.SameAs(version));
            Assert.That(builtVersion.ServicePacks, Is.SameAs(servicePacks));
            Assert.That(builtVersion.Profiles, Is.EqualTo(profiles));

            // Fixture teardown by GC...
        }
    }
}