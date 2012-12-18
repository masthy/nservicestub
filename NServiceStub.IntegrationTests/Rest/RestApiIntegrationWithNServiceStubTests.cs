using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using NServiceStub.Configuration;
using NServiceStub.NServiceBus;
using NServiceStub.Rest;
using NUnit.Framework;
using OrderService.Contracts;
using NServiceStub.Rest.Configuration;

namespace NServiceStub.IntegrationTests.Rest
{
    [TestFixture]
    public class RestApiIntegrationWithNServiceStubTests
    {
        [Test]
        public void SimpleExpectationSetUp_InvokingEndpointAndExpectationMet_MessageIsSentToQueue()
        {
            MsmqHelpers.Purge("shippingservice");

            var service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            string baseUrl = "http://localhost:9101/";
            var proxy = service.RestEndpoint(baseUrl);

            IRouteDefinition<bool> get = proxy.AddRouteGet<bool>("/list");

            proxy.Configure(get).Setup(Parameter.Any()).Returns(true)
                .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");

            service.Start();

            var client = new HttpClient();
            client.BaseAddress = new Uri(baseUrl);

            Task<string> getAsync = client.GetStringAsync("list");

            getAsync.Wait();
            string body = getAsync.Result;

            do
            {
                Thread.Sleep(100);
            } while (MsmqHelpers.GetMessageCount("shippingservice") == 0);

            client.Dispose();
            service.Dispose();

            Assert.That(body, Is.EqualTo("true"));
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(1), "shipping service did not recieve send");
        }
    }
}