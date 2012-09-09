using DotNetDetector;
using NUnit.Framework;
using Rhino.Mocks;

namespace DotNetDetectorTests
{
    [TestFixture]
    public class RegistryKeyBaseTests
    {
        [Test]
        public void MatchRegistryValue_NotSubKey_Null()
        {
            // Fixture setup...
            const string subKeyName = "subKey";

            var key = MockRepository.GeneratePartialMock<RegistryKeyBase>();
            key.Expect(k => k.OpenSubKey(subKeyName)).Return(null);

            // Exercise SUT...
            var res = key.MatchRegistryValue(subKeyName, null, null);

            // Verify SUT...
            key.VerifyAllExpectations();
            Assert.That(res, Is.False);

            // Fixture teardown by GC...
        }

        [Test]
        public void MatchRegistryValue_NotMatch_False()
        {
            // Fixture setup...
            const string subKeyName = "subKey";
            const string valueName = "valueName";
            const int value = 0;

            var subKey = MockRepository.GeneratePartialMock<RegistryKeyBase>();
            subKey.Expect(s => s.GetValue(valueName)).Return(value + 1);
            subKey.Expect(s => s.Dispose());

            var key = MockRepository.GeneratePartialMock<RegistryKeyBase>();
            key.Expect(k => k.OpenSubKey(subKeyName)).Return(subKey);

            // Exercise SUT...
            var res = key.MatchRegistryValue(subKeyName, valueName, value);

            // Verify SUT...
            subKey.VerifyAllExpectations();
            key.VerifyAllExpectations();
            Assert.That(res, Is.False);

            // Fixture teardown by GC...
        }

        [Test]
        public void MatchRegistryValue_Match_True()
        {
            // Fixture setup...
            const string subKeyName = "subKey";
            const string valueName = "valueName";
            const int value = 0;

            var subKey = MockRepository.GeneratePartialMock<RegistryKeyBase>();
            subKey.Expect(s => s.GetValue(valueName)).Return(value);
            subKey.Expect(s => s.Dispose());

            var key = MockRepository.GeneratePartialMock<RegistryKeyBase>();
            key.Expect(k => k.OpenSubKey(subKeyName)).Return(subKey);

            // Exercise SUT...
            var res = key.MatchRegistryValue(subKeyName, valueName, value);

            // Verify SUT...
            subKey.VerifyAllExpectations();
            key.VerifyAllExpectations();
            Assert.That(res, Is.True);

            // Fixture teardown by GC...
        }
    }
}
