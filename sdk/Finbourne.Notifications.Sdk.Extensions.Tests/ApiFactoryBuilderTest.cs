using NUnit.Framework;

namespace Finbourne.Notifications.Sdk.Extensions.Tests
{
    [TestFixture]
    public class ApiFactoryBuilderTest
    {
        [Test]
        public void Build_From_Secrets_Returns_NonNull_ApiFactory()
        {
            var apiConfig = ApiConfigurationBuilder.Build("dummy-test-secrets.json");
            var apiFactory = new ApiFactory(apiConfig);
            Assert.IsNotNull(apiFactory);
        }

        //Test requires [assembly: InternalsVisibleTo("Finbourne.Notifications.Sdk.Extensions.Tests")] in ClientCredentialsFlowTokenProvider
        [Test]
        public void Build_From_Configuration_Returns_NonNull_ApiFactory()
        {
            var config = new TokenProviderConfiguration(new ClientCredentialsFlowTokenProvider(ApiConfigurationBuilder.Build("dummy-test-secrets.json")))
            {
                BasePath = "base path"
            };
            var apiFactory = new ApiFactory(config);
            Assert.IsNotNull(apiFactory);
        }
    }
}
