using NUnit.Framework;
using Rhino.Mocks;
using DotNetDetector;

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
            Assert.That(Detector.Current, Is.EqualTo(current));

            // Fixture teardown by GC...
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
    }
}
