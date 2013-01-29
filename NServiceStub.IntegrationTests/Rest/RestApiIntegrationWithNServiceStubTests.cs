using System;
using System.Net;
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
        public void Get_InvokingEndpointAndExpectationMet_MessageIsSentToQueue()
        {
            MsmqHelpers.Purge("shippingservice");

            var service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            var restEndpoint = service.RestEndpoint(BaseUrl);

            IGetTemplate<bool> get = restEndpoint.AddGet<bool>("/list");

            restEndpoint.Configure(get).With(Parameter.Any()).Returns(true)
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
        public void Get_SimpleExpectationSetUpInHeaderInvokingEndpointAndExpectationMet_MessageIsSentToQueue()
        {
            MsmqHelpers.Purge("shippingservice");

            var service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            var restEndpoint = service.RestEndpoint(BaseUrl);

            IGetTemplate<bool> get = restEndpoint.AddGet<bool>("/list");

            restEndpoint.Configure(get).With(Parameter.HeaderParameter<DateTime>("Today").Equals(dt => dt.Day == 3)).Returns(true)
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
        public void Get_SimpleExpectationSetUpInHeaderInvokingEndpointAndExpectationAreNotMet_MessageIsNotSentToQueue()
        {
            MsmqHelpers.Purge("shippingservice");

            var service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            var restEndpoint = service.RestEndpoint(BaseUrl);

            IGetTemplate<bool> get = restEndpoint.AddGet<bool>("/list");

            restEndpoint.Configure(get).With(Parameter.HeaderParameter<DateTime>("Today").Equals(dt => dt.Day == 3)).Returns(true)
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
        public void Get_SimpleExpectationSetUpInHeaderHeaderVariableIsMissing_ReturnsDefaultValue()
        {
            MsmqHelpers.Purge("shippingservice");

            var service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            var restEndpoint = service.RestEndpoint(BaseUrl);

            IGetTemplate<bool> get = restEndpoint.AddGet<bool>("/list");

            restEndpoint.Configure(get).With(Parameter.HeaderParameter<DateTime>("Today").Equals(dt => dt.Day == 3)).Returns(true)
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
        public void Get_SimpleExpectationSetUpInvokingEndpointAndExpectationMet_InputParametersArePassedDownTheChain()
        {
            MsmqHelpers.Purge("shippingservice");

            var service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            var restEndpoint = service.RestEndpoint(BaseUrl);

            IGetTemplate<bool> get = restEndpoint.AddGet<bool>("/order/{id}");

            restEndpoint.Configure(get).With(Parameter.RouteParameter<int>("id").Equals(1))
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
        public void Get_SimpleExpectationSetUpInvokingEndpointAndExpectationMetInDifferentOrder_RestApiReturnsMatch()
        {
            var service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            var restEndpoint = service.RestEndpoint(BaseUrl);

            IGetTemplate<bool> get = restEndpoint.AddGet<bool>("/order/{id}?foo&bar");

            restEndpoint.Configure(get).With(Parameter.RouteParameter<int>("id").Equals(1)
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

        [Test]
        public void Post_JustASimplePostWithNoBody_MessagesAreSent()
        {
            // Arrange
            MsmqHelpers.Purge("shippingservice");

            var service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            var api = service.RestEndpoint(BaseUrl);

            IPostTemplate post = api.AddPost("/order/{id}/shares");
            api.Configure(post).With(Parameter.RouteParameter<int>("id").Equals(1)).Send<IOrderWasPlaced>(msg =>
                {
                    msg.OrderNumber = 1;
                }, "shippingservice");

            service.Start();

            var client = new HttpClient { BaseAddress = new Uri(BaseUrl) };

            Task<HttpResponseMessage> postAsync = client.PostAsync("/order/1/shares", new StringContent(""));

            postAsync.Wait();
            var message = postAsync.Result;

            client.Dispose();
            service.Dispose();

            do
            {
                Thread.Sleep(100);
            } while (MsmqHelpers.GetMessageCount("shippingservice") == 0);

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(1));
        }

        [Test]
        public void Post_DoesNotMatchSetup_DoesNotSendMessages()
        {
            // Arrange
            MsmqHelpers.Purge("shippingservice");

            var service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            var api = service.RestEndpoint(BaseUrl);

            IPostTemplate post = api.AddPost("/order/{id}/shares");
            api.Configure(post).With(Parameter.RouteParameter<int>("id").Equals(1)).Send<IOrderWasPlaced>(msg =>
            {
                msg.OrderNumber = 1;
            }, "shippingservice");

            service.Start();

            var client = new HttpClient { BaseAddress = new Uri(BaseUrl) };

            Task<HttpResponseMessage> postAsync = client.PostAsync("/order/2/shares", new StringContent(""));

            postAsync.Wait();

            client.Dispose();
            service.Dispose();

            Thread.Sleep(2000);
            
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(0), "shipping service recieved events");
        }
    }
}