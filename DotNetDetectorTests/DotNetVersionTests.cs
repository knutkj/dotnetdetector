using System;
using System.Collections.Generic;
using System.Linq;
using DotNetDetector;
using NUnit.Framework;

namespace DotNetDetectorTests
{
    [TestFixture]
    public class DotNetVersionTests
    {
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void CtorNoVersionException()
        {
            // Fixture setup...

            // Exercise and verify SUT...
            new DotNetVersion(
                null,
                new Version[0],
                DotNetProfiles.Client
            );

            // Fixture teardown by GC...
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void CtorNoPacksException()
        {
            // Fixture setup...

            // Exercise and verify SUT...
            new DotNetVersion(
                new Version(),
                null,
                DotNetProfiles.Client
            );

            // Fixture teardown by GC...
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void CtorNoProfileException()
        {
            // Fixture setup...

            // Exercise and verify SUT...
            new DotNetVersion(
                new Version(),
                0
            );

            // Fixture teardown by GC...
        }

        [Test]
        public void CtorStoresArgs()
        {
            // Fixture setup...
            var version = new Version();
            var packs = new Version[0];
            var profiles = DotNetProfiles.Full;

            // Exercise and SUT...
            var netVersion = new DotNetVersion(version, packs, profiles);

            // Verify SUT...
            Assert.That(netVersion.Version, Is.EqualTo(version));
            Assert.That(netVersion.ServicePacks, Is.EqualTo(packs));
            Assert.That(netVersion.Profiles, Is.EqualTo(profiles));

            // Fixture teardown by GC...
        }

        [Test]
        public void CtorWithoutProfileDefaultsToAllProfiles()
        {
            // Fixture setup...

            // Exercise SUT...
            var version = new DotNetVersion(new Version(), new Version[0]);

            // Verify SUT...
            Assert.That(
                version.Profiles,
                Is.EqualTo(DotNetProfiles.ClientFull)
            );

            // Fixture teardown by GC...
        }

        [Test]
        public void CtorWithoutPacksDefaultsToEmptyPackList()
        {
            // Fixture setup...

            // Exercise SUT...
            var version = new DotNetVersion(
                new Version(),
                DotNetProfiles.Client
            );

            // Verify SUT...
            Assert.That(
                version.ServicePacks,
                Is.InstanceOf<IEnumerable<Version>>()
            );
            Assert.That(version.ServicePacks.Count(), Is.EqualTo(0));

            // Fixture teardown by GC...
        }

        [Test]
        public void CtorWithoutPacksAndProfilesDefaults()
        {
            // Fixture setup...

            // Exercise SUT...
            var version = new DotNetVersion(new Version());

            // Verify SUT...
            Assert.That(
                version.ServicePacks,
                Is.InstanceOf<IEnumerable<Version>>()
            );
            Assert.That(version.ServicePacks.Count(), Is.EqualTo(0));
            Assert.That(
                version.Profiles,
                Is.EqualTo(DotNetProfiles.ClientFull)
            );

            // Fixture teardown by GC...
        }

        [TestCase(
            "4.0", "1.0", DotNetProfiles.ClientFull,
            "Microsoft .NET Framework 4.0 Service Pack 1 Full Profile"
        )]
        [TestCase(
            "1.0", "", DotNetProfiles.Client,
            "Microsoft .NET Framework 1.0 Client Profile"
        )]
        [TestCase(
            "3.5", "1.0,2.0,3.0", DotNetProfiles.Full,
            "Microsoft .NET Framework 3.5 Service Pack 3 Full Profile"
        )]
        public void ToStringNiceRepresentation(
            string v,
            string sp,
            DotNetProfiles p,
            string res
        )
        {
            // Fixture setup...

            // Exercise SUT...
            var version = new DotNetVersion(
                new Version(v),
                string.IsNullOrWhiteSpace(sp) ?
                    new Version[0] :
                    sp.Split(new[] { ',' }).Select(spv => new Version(spv)),
                p
            );

            // Verify SUT...
            Assert.That(version.ToString(), Is.EqualTo(res));

            // Fixture teardown by GC...
        }
    }
}
