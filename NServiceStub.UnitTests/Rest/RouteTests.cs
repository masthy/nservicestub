using System.Collections.Generic;
using System.Text.RegularExpressions;
using NServiceStub.Rest;
using NUnit.Framework;

namespace NServiceStub.UnitTests.Rest
{
    [TestFixture]
    public class RouteTests
    {
        [Test]
        public void Matches_NumberOfParametersMatchButNotAllParametersProvided_DoesNotMatch()
        {
            // Arrange
            var route = new Route(new Regex("^/order/(?<id>[^/]+)\\?(?<param0>[^=\\?&]+)=(?<value0>[^&]+)&(?<param1>[^=\\?&]+)=(?<value1>[^&]+)$"), new Dictionary<string, string> { { "id", "id" } }, new List<string> { "foo", "bar" }, "param", "value");
            
            // Act
            bool matches = route.Matches("/order/1?bar=partner&bar=partner");

            // Assert
            Assert.That(matches, Is.False);
        } 
    }
}