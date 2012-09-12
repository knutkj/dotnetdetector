using System;
using DotNetDetector;
using DotNetDetectorApp;
using NUnit.Framework;
using Rhino.Mocks;

namespace DotNetDetectorTests
{
    [TestFixture]
    public class DotNetVersionWriterTests
    {
        [Test]
        public void WriteAll()
        {
            // Fixture setup...
            var version = new DotNetVersion(new Version("1.0"));
            var versions = new[] { version };

            var detector = MockRepository.GenerateMock<IDetector>();
            detector.Expect(d => d.Versions).Return(versions);

            var writer = MockRepository
                .GeneratePartialMock<DotNetVersionWriter>(detector);
            writer.Expect(w => w.Write(version));

            // Exercise SUT...
            writer.WriteAll();

            // Verify SUT...
            detector.VerifyAllExpectations();
            writer.VerifyAllExpectations();

            // Fixture teardown by GC...
        }
    }
}