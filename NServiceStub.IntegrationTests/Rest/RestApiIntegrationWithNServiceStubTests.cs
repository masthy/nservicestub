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

            const string BaseUrl = "http://localhost:9101/";
            var restEndpoint = service.RestEndpoint(BaseUrl);

            IRouteDefinition<bool> get = restEndpoint.AddRouteGet<bool>("/list");

            restEndpoint.Configure(get).Setup(Parameter.Any()).Returns(true)
                .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");

            service.Start();

            var client = new HttpClient();
            client.BaseAddress = new Uri(BaseUrl);

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

        [Test]
        public void SimpleExpectationSetUp_InvokingEndpointAndExpectationMet_InputParametersArePassedDownTheChain()
        {
            MsmqHelpers.Purge("shippingservice");

            var service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            var restEndpoint = service.RestEndpoint(BaseUrl);

            IRouteDefinition<bool> get = restEndpoint.AddRouteGet<bool>("/order/{id}");

            restEndpoint.Configure(get).Setup(Parameter.RouteParameter<int>("id").Equals(1))
                .Returns(true)
                .Send<IOrderWasPlaced, int>((msg, id) =>
                    {
                        msg.OrderedProduct = "stockings";
                        msg.OrderNumber = id;
                    }, "shippingservice");

            service.Start();

            var client = new HttpClient {BaseAddress = new Uri(BaseUrl)};

            Task<string> getAsync = client.GetStringAsync("/order/1");

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