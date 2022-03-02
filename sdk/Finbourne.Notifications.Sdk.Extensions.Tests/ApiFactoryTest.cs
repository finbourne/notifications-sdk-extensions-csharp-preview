using NUnit.Framework;
using System;

namespace Finbourne.Notifications.Sdk.Extensions.Tests
{
    [TestFixture]
    public class ApiFactoryTest
    {
        [Test]
        public void InvalidTokenUrl_ThrowsException()
        {
            ApiConfiguration apiConfig = new ApiConfiguration
            {
                TokenUrl = "xyz"
            };

            Assert.That(
                () => new ApiFactory(apiConfig),
                Throws.InstanceOf<UriFormatException>().With.Message.EqualTo("Invalid Token Uri: xyz"));
        }

        [Test]
        public void InvalidNotificationsUrl_ThrowsException()
        {
            ApiConfiguration apiConfig = new ApiConfiguration
            {
                TokenUrl = "http://finbourne.com",
                NotificationsUrl = "xyz"
            };

            Assert.That(
                () => new ApiFactory(apiConfig),
                Throws.InstanceOf<UriFormatException>().With.Message.EqualTo("Invalid Uri: xyz"));
        }
    }
}
