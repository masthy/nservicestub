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

            var client = new HttpClient {BaseAddress = new Uri(BaseUrl)};

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
        public void SimpleExpectationSetUpInHeader_InvokingEndpointAndExpectationMet_MessageIsSentToQueue()
        {
            MsmqHelpers.Purge("shippingservice");

            var service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            var restEndpoint = service.RestEndpoint(BaseUrl);

            IRouteDefinition<bool> get = restEndpoint.AddRouteGet<bool>("/list");

            restEndpoint.Configure(get).Setup(Parameter.HeaderParameter<DateTime>("Today").Equals(dt => dt.Day == 3)).Returns(true)
                .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");

            service.Start();

            var client = new HttpClient {BaseAddress = new Uri(BaseUrl)};

            client.DefaultRequestHeaders.Add("Today", new DateTime(2000, 2, 3).ToString());

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
        public void SimpleExpectationSetUpInHeader_InvokingEndpointAndExpectationAreNotMet_MessageIsNotSentToQueue()
        {
            MsmqHelpers.Purge("shippingservice");

            var service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            var restEndpoint = service.RestEndpoint(BaseUrl);

            IRouteDefinition<bool> get = restEndpoint.AddRouteGet<bool>("/list");

            restEndpoint.Configure(get).Setup(Parameter.HeaderParameter<DateTime>("Today").Equals(dt => dt.Day == 3)).Returns(true)
                .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");

            service.Start();

            var client = new HttpClient { BaseAddress = new Uri(BaseUrl) };

            client.DefaultRequestHeaders.Add("Today", new DateTime(2000, 2, 4).ToString());

            Task<string> getAsync = client.GetStringAsync("list");

            getAsync.Wait();
            string body = getAsync.Result;

            client.Dispose();
            service.Dispose();

            Assert.That(body, Is.EqualTo("null"));
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(0), "shipping service recieved events");
        }

        [Test]
        public void SimpleExpectationSetUpInHeader_HeaderVariableIsMissing_ReturnsDefaultValue()
        {
            MsmqHelpers.Purge("shippingservice");

            var service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            var restEndpoint = service.RestEndpoint(BaseUrl);

            IRouteDefinition<bool> get = restEndpoint.AddRouteGet<bool>("/list");

            restEndpoint.Configure(get).Setup(Parameter.HeaderParameter<DateTime>("Today").Equals(dt => dt.Day == 3)).Returns(true)
                .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");

            service.Start();

            var client = new HttpClient { BaseAddress = new Uri(BaseUrl) };

            Task<string> getAsync = client.GetStringAsync("list");

            getAsync.Wait();
            string body = getAsync.Result;

            client.Dispose();
            service.Dispose();

            Assert.That(body, Is.EqualTo("null"));
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(0), "shipping service recieved events");
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

        [Test]
        public void SimpleExpectationSetUp_InvokingEndpointAndExpectationMetInDifferentOrder_RestApiReturnsMatch()
        {
            var service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            var restEndpoint = service.RestEndpoint(BaseUrl);

            IRouteDefinition<bool> get = restEndpoint.AddRouteGet<bool>("/order/{id}?foo&bar");

            restEndpoint.Configure(get).Setup(Parameter.RouteParameter<int>("id").Equals(1)
                .And(Parameter.QueryParameter<string>("foo").Equals("howdy"))
                .And(Parameter.QueryParameter<string>("bar").Equals("partner"))).Returns(true);

            service.Start();

            var client = new HttpClient { BaseAddress = new Uri(BaseUrl) };

            Task<string> getAsync = client.GetStringAsync("/order/1?bar=partner&foo=howdy");

            getAsync.Wait();
            string body = getAsync.Result;

            client.Dispose();
            service.Dispose();

            Assert.That(body, Is.EqualTo("true"));
        }
    }
}