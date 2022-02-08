using NUnit.Framework;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Finbourne.Notifications.Sdk.Extensions.Tests
{
    [TestFixture]
    public class ClientCredentialsFlowTokenProviderTest
    {
        [Test]
        public void Constructor_NonNull_Instance_Returned()
        {
            var tokenProvider = new ClientCredentialsFlowTokenProvider(ApiConfigurationBuilder.Build("dummy-test-secrets.json"));
            Assert.IsNotNull(tokenProvider);
        }
    }
}
