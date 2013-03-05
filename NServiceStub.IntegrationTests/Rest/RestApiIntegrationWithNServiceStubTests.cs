using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NServiceStub.Configuration;
using NServiceStub.NServiceBus;
using NServiceStub.Rest;
using NServiceStub.Rest.Configuration;
using NUnit.Framework;
using OrderService.Contracts;

namespace NServiceStub.IntegrationTests.Rest
{
    [TestFixture]
    public class RestApiIntegrationWithNServiceStubTests
    {
        [Test]
        public void Get_InvokingEndpointAndExpectationMet_MessageIsSentToQueue()
        {
            MsmqHelpers.Purge("shippingservice");

            ServiceStub service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            RestApi restEndpoint = service.RestEndpoint(BaseUrl);

            IRouteTemplate<bool> get = restEndpoint.AddGet<bool>("/list");

            restEndpoint.Configure(get).With(Parameter.Any()).Returns(true)
                        .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");

            service.Start();

            var client = new HttpClient {BaseAddress = new Uri(BaseUrl)};

            Task<string> getAsync = client.GetStringAsync("list");

            string result = WaitVerifyNoExceptionsAndGetResult(getAsync);

            do
            {
                Thread.Sleep(100);
            } while (MsmqHelpers.GetMessageCount("shippingservice") == 0);

            client.Dispose();
            service.Dispose();

            Assert.That(result, Is.EqualTo("true"));
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(1), "shipping service did not recieve send");
        }

        [Test]
        public void Get_SimpleExpectationSetUpInHeaderHeaderVariableIsMissing_ReturnsDefaultValue()
        {
            MsmqHelpers.Purge("shippingservice");

            ServiceStub service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            RestApi restEndpoint = service.RestEndpoint(BaseUrl);

            IRouteTemplate<bool> get = restEndpoint.AddGet<bool>("/list");

            restEndpoint.Configure(get).With(Parameter.HeaderParameter<DateTime>("Today").Equals(dt => dt.Day == 3)).Returns(true)
                        .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");

            service.Start();

            var client = new HttpClient {BaseAddress = new Uri(BaseUrl)};

            Task<string> getAsync = client.GetStringAsync("list");

            string result = WaitVerifyNoExceptionsAndGetResult(getAsync);

            client.Dispose();
            service.Dispose();

            Assert.That(result, Is.EqualTo("null"));
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(0), "shipping service recieved events");
        }

        [Test]
        public void Get_SimpleExpectationSetUpInHeader_HeaderParameterUsableInReturnStatement()
        {
            MsmqHelpers.Purge("shippingservice");

            ServiceStub service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            RestApi restEndpoint = service.RestEndpoint(BaseUrl);

            IRouteTemplate<SomeTupleReturnValue> get = restEndpoint.AddGet<SomeTupleReturnValue>("/list/{id}");

            restEndpoint.Configure(get).With(Parameter.HeaderParameter<DateTime>("Today").Equals(dt => dt.Day == 3).And(Parameter.RouteParameter<int>("id").Any())).Returns<DateTime, int>((today, id) => new SomeTupleReturnValue{ One = today, Two = id});

            service.Start();

            var client = new HttpClient { BaseAddress = new Uri(BaseUrl) };

            client.DefaultRequestHeaders.Add("Today", new DateTime(2000, 1, 3).ToString());

            Task<string> getAsync = client.GetStringAsync("list/1");

            string result = WaitVerifyNoExceptionsAndGetResult(getAsync);

            client.Dispose();
            service.Dispose();

            Assert.That(result, Is.EqualTo("{\"One\":\"2000-01-03T00:00:00\",\"Two\":1}"));
        }

        [Test]
        public void Get_SimpleExpectationSetUpInHeader_HeaderParameterPassedDownTheInheritanceChain()
        {
            MsmqHelpers.Purge("shippingservice");

            ServiceStub service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            RestApi restEndpoint = service.RestEndpoint(BaseUrl);

            IRouteTemplate<bool> get = restEndpoint.AddGet<bool>("/list");

            restEndpoint.Configure(get).With(Parameter.HeaderParameter<DateTime>("Today").Equals(dt => dt.Day == 3)).Returns(true)
                        .Send<IOrderWasPlaced, DateTime>((msg, today) => msg.OrderedProduct = today.ToString(), "shippingservice");

            service.Start();

            var client = new HttpClient { BaseAddress = new Uri(BaseUrl) };

            client.DefaultRequestHeaders.Add("Today", new DateTime(2000, 1, 3).ToString());

            Task<string> getAsync = client.GetStringAsync("list");

            string result = WaitVerifyNoExceptionsAndGetResult(getAsync);

            client.Dispose();
            service.Dispose();

            do
            {
                Thread.Sleep(100);
            } while (MsmqHelpers.GetMessageCount("shippingservice") == 0);

            Assert.That(result, Is.EqualTo("true"));
            Assert.That(MsmqHelpers.PickMessageBody("shippingservice"), Is.StringContaining(new DateTime(2000, 1, 3).ToString()), "shipping service recieved events");
        }

        [Test]
        public void Get_SimpleExpectationSetUpInHeaderInvokingEndpointAndExpectationAreNotMet_MessageIsNotSentToQueue()
        {
            MsmqHelpers.Purge("shippingservice");

            ServiceStub service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            RestApi restEndpoint = service.RestEndpoint(BaseUrl);

            IRouteTemplate<bool> get = restEndpoint.AddGet<bool>("/list");

            restEndpoint.Configure(get).With(Parameter.HeaderParameter<DateTime>("Today").Equals(dt => dt.Day == 3)).Returns(true)
                        .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");

            service.Start();

            var client = new HttpClient {BaseAddress = new Uri(BaseUrl)};

            client.DefaultRequestHeaders.Add("Today", new DateTime(2000, 2, 4).ToString());

            Task<string> getAsync = client.GetStringAsync("list");

            string result = WaitVerifyNoExceptionsAndGetResult(getAsync);

            client.Dispose();
            service.Dispose();

            Assert.That(result, Is.EqualTo("null"));
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(0), "shipping service recieved events");
        }

        [Test]
        public void Get_SimpleExpectationSetUpInHeaderInvokingEndpointAndExpectationMet_MessageIsSentToQueue()
        {
            MsmqHelpers.Purge("shippingservice");

            ServiceStub service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            RestApi restEndpoint = service.RestEndpoint(BaseUrl);

            IRouteTemplate<bool> get = restEndpoint.AddGet<bool>("/list");

            restEndpoint.Configure(get).With(Parameter.HeaderParameter<DateTime>("Today").Equals(dt => dt.Day == 3)).Returns(true)
                        .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");

            service.Start();

            var client = new HttpClient {BaseAddress = new Uri(BaseUrl)};

            client.DefaultRequestHeaders.Add("Today", new DateTime(2000, 2, 3).ToString());

            Task<string> getAsync = client.GetStringAsync("list");

            string result = WaitVerifyNoExceptionsAndGetResult(getAsync);

            do
            {
                Thread.Sleep(100);
            } while (MsmqHelpers.GetMessageCount("shippingservice") == 0);

            client.Dispose();
            service.Dispose();

            Assert.That(result, Is.EqualTo("true"));
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(1), "shipping service did not recieve send");
        }

        [Test]
        public void Get_SimpleExpectationSetUpInvokingEndpointAndExpectationMetInDifferentOrder_RestApiReturnsMatch()
        {
            ServiceStub service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            RestApi restEndpoint = service.RestEndpoint(BaseUrl);

            IRouteTemplate<bool> get = restEndpoint.AddGet<bool>("/order/{id}?foo&bar");

            restEndpoint.Configure(get).With(Parameter.RouteParameter<int>("id").Equals(1)
                                                      .And(Parameter.QueryParameter<string>("foo").Equals("howdy"))
                                                      .And(Parameter.QueryParameter<string>("bar").Equals("partner"))).Returns(true);

            service.Start();

            var client = new HttpClient {BaseAddress = new Uri(BaseUrl)};

            Task<string> getAsync = client.GetStringAsync("/order/1?bar=partner&foo=howdy");

            string result = WaitVerifyNoExceptionsAndGetResult(getAsync);

            client.Dispose();
            service.Dispose();

            Assert.That(result, Is.EqualTo("true"));
        }

        [Test]
        public void Get_SimpleExpectationSetUpInvokingEndpointAndExpectationMet_InputParametersArePassedDownTheChain()
        {
            MsmqHelpers.Purge("shippingservice");

            ServiceStub service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            RestApi restEndpoint = service.RestEndpoint(BaseUrl);

            IRouteTemplate<bool> get = restEndpoint.AddGet<bool>("/order/{id}");

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

            string result = WaitVerifyNoExceptionsAndGetResult(getAsync);

            do
            {
                Thread.Sleep(100);
            } while (MsmqHelpers.GetMessageCount("shippingservice") == 0);

            client.Dispose();
            service.Dispose();

            Assert.That(result, Is.EqualTo("true"));
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(1), "shipping service did not recieve send");
        }

        [Test]
        public void Post_DoesNotMatchSetup_DoesNotSendMessages()
        {
            // Arrange
            MsmqHelpers.Purge("shippingservice");

            ServiceStub service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            RestApi api = service.RestEndpoint(BaseUrl);

            IRouteTemplate post = api.AddPost("/order/{id}/shares");
            api.Configure(post).With(Parameter.RouteParameter<int>("id").Equals(1)).Send<IOrderWasPlaced>(msg => { msg.OrderNumber = 1; }, "shippingservice");

            service.Start();

            var client = new HttpClient {BaseAddress = new Uri(BaseUrl)};

            Task<HttpResponseMessage> postAsync = client.PostAsync("/order/2/shares", new StringContent(""));

            WaitVerifyNoExceptions(postAsync);

            client.Dispose();
            service.Dispose();

            Thread.Sleep(2000);

            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(0), "shipping service recieved events");
        }

        [Test]
        public void Post_JustASimplePostWithNoBody_MessagesAreSent()
        {
            // Arrange
            MsmqHelpers.Purge("shippingservice");

            ServiceStub service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            RestApi api = service.RestEndpoint(BaseUrl);

            IRouteTemplate post = api.AddPost("/order/{id}/shares");
            api.Configure(post).With(Parameter.RouteParameter<int>("id").Equals(1)).Send<IOrderWasPlaced>(msg => { msg.OrderNumber = 1; }, "shippingservice");

            service.Start();

            var client = new HttpClient {BaseAddress = new Uri(BaseUrl)};

            Task<HttpResponseMessage> postAsync = client.PostAsync("/order/1/shares", new StringContent(""));

            HttpResponseMessage message = WaitVerifyNoExceptionsAndGetResult(postAsync);

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
        public void Post_PostWitBody_BodyIsBoundToDynamic()
        {
            MsmqHelpers.Purge("shippingservice");

            ServiceStub service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            RestApi api = service.RestEndpoint(BaseUrl);

            IRouteTemplate post = api.AddPost("/order");
            api.Configure(post).With(Body.AsDynamic().IsEqualTo(body => body.orderId == 1)).Returns(2);

            service.Start();

            var client = new HttpClient {BaseAddress = new Uri(BaseUrl)};

            Task<HttpResponseMessage> postAsync = client.PostAsync("/order", new StringContent("{\"orderId\":\"1\"}", Encoding.UTF8, "application/json"));

            HttpResponseMessage message = WaitVerifyNoExceptionsAndGetResult(postAsync);

            Task<string> readAsStringAsync = message.Content.ReadAsStringAsync();
            readAsStringAsync.Wait();

            client.Dispose();
            service.Dispose();

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(readAsStringAsync.Result, Is.EqualTo("2"));
        }

        [Test]
        public void Post_PostWitBodyAndRouteParameters_BodyAndParametersFromRequestAreBound()
        {
            // Arrange
            MsmqHelpers.Purge("shippingservice");

            ServiceStub service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            RestApi api = service.RestEndpoint(BaseUrl);

            IRouteTemplate post = api.AddPost("/order/{id}");
            api.Configure(post).With(Body.AsDynamic().IsEqualTo(body => body.orderId == 1).And(Parameter.RouteParameter<int>("id").Equals(2))).Returns(3);

            service.Start();

            var client = new HttpClient { BaseAddress = new Uri(BaseUrl) };

            Task<HttpResponseMessage> postAsync = client.PostAsync("/order/2", new StringContent("{\"orderId\":\"1\"}", Encoding.UTF8, "application/json"));

            HttpResponseMessage message = WaitVerifyNoExceptionsAndGetResult(postAsync);

            Task<string> readAsStringAsync = message.Content.ReadAsStringAsync();
            readAsStringAsync.Wait();

            client.Dispose();
            service.Dispose();

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(readAsStringAsync.Result, Is.EqualTo("3"));
        }

        [Test]
        public void Post_PostWitBodyAndQueryParameters_BodyAndParametersFromRequestAreBound()
        {
            // Arrange
            MsmqHelpers.Purge("shippingservice");

            ServiceStub service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            RestApi api = service.RestEndpoint(BaseUrl);

            IRouteTemplate post = api.AddPost("/order?id&size");
            api.Configure(post).With(Body.AsDynamic().IsEqualTo(body => body.orderId == 1)
                .And(Parameter.QueryParameter<int>("id").Equals(2))
                .And(Parameter.QueryParameter<int>("size").Equals(1))).Returns(3);

            service.Start();

            var client = new HttpClient { BaseAddress = new Uri(BaseUrl) };

            Task<HttpResponseMessage> postAsync = client.PostAsync("/order?id=2&size=1", new StringContent("{\"orderId\":\"1\"}", Encoding.UTF8, "application/json"));

            HttpResponseMessage message = WaitVerifyNoExceptionsAndGetResult(postAsync);

            Task<string> readAsStringAsync = message.Content.ReadAsStringAsync();
            readAsStringAsync.Wait();

            client.Dispose();
            service.Dispose();

            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(readAsStringAsync.Result, Is.EqualTo("3"));
        }

        [Test]
        public void Post_PostWitBody_BodyIsPassedDownTheChain()
        {
            MsmqHelpers.Purge("shippingservice");

            ServiceStub service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            RestApi api = service.RestEndpoint(BaseUrl);

            IRouteTemplate post = api.AddPost("/order");
            api.Configure(post).With(Body.AsDynamic().IsEqualTo(body => body.orderId == 1))
                .Send<IOrderWasPlaced, dynamic>((msg, body) =>
                    {
                        msg.OrderNumber = body.orderId;
                    }, "shippingservice");

            service.Start();

            var client = new HttpClient { BaseAddress = new Uri(BaseUrl) };

            Task<HttpResponseMessage> postAsync = client.PostAsync("/order", new StringContent("{\"orderId\":\"1\"}", Encoding.UTF8, "application/json"));

            HttpResponseMessage message = WaitVerifyNoExceptionsAndGetResult(postAsync);

            MsmqHelpers.WaitForMessages("shippingservice");

            client.Dispose();
            service.Dispose();

            Assert.That(MsmqHelpers.PickMessageBody("shippingservice"), Is.StringContaining("1"));
            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void Post_PostWitBodyAndRouteParameter_BodyAndRouteParameterIsPassedDownTheChain()
        {
            MsmqHelpers.Purge("shippingservice");

            ServiceStub service = Configure.Stub().NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            const string BaseUrl = "http://localhost:9101/";
            RestApi api = service.RestEndpoint(BaseUrl);

            IRouteTemplate post = api.AddPost("/order/{id}");

            api.Configure(post).With(Body.AsDynamic().IsEqualTo(body => body.product == "socks"))
                .Send<IOrderWasPlaced, dynamic, int>((msg, body, id) =>
                {
                    msg.OrderedProduct = body.product;
                    msg.OrderNumber = id;
                }, "shippingservice");

            service.Start();

            var client = new HttpClient { BaseAddress = new Uri(BaseUrl) };

            Task<HttpResponseMessage> postAsync = client.PostAsync("/order/2", new StringContent("{\"product\":\"socks\"}", Encoding.UTF8, "application/json"));

            HttpResponseMessage message = WaitVerifyNoExceptionsAndGetResult(postAsync);

            MsmqHelpers.WaitForMessages("shippingservice");

            client.Dispose();
            service.Dispose();

            object actual = MsmqHelpers.PickMessageBody("shippingservice");

            Assert.That(actual, Is.StringContaining("socks"));
            Assert.That(actual, Is.StringContaining("2"));
            Assert.That(message.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        private static T WaitVerifyNoExceptionsAndGetResult<T>(Task<T> task)
        {
            WaitVerifyNoExceptions(task);

            T result = task.Result;
            return result;
        }

        private static void WaitVerifyNoExceptions(Task task)
        {
            task.Wait();

            Assert.That(task.Exception, Is.Null);
        }
    }
}