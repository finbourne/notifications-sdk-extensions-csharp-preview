using NUnit.Framework;

namespace Finbourne.Notifications.Sdk.Extensions.Tests
{
    [TestFixture]
    public class TokenProviderConfigurationTest
    {
        private const string APP = "notifications";

        [Test]
        public void Construct_WithNullTokenProvider_Returns_NonNull()
        {
            var config = new TokenProviderConfiguration(null);
            Assert.IsNotNull(config);
        }

        [Test]
        public void Construct_WithNullTokenProvider_Returns_BasePathSet()
        {
            var config = new TokenProviderConfiguration(null);
            StringAssert.AreEqualIgnoringCase($"https://www.lusid.com/{APP}", config.BasePath);
        }
    }
}
