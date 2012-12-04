using System.ServiceModel;
using System.Threading;
using NServiceStub.Configuration;
using NServiceStub.NServiceBus;
using NServiceStub.WCF;
using NServiceStub.WCF.Configuration;
using NUnit.Framework;
using OrderService.Contracts;

namespace NServiceStub.IntegrationTests.WCF
{
    [TestFixture]
    public class WcfProxyIntegrationWithNServiceStubTests
    {
        [Test]
        public void SimpleExpectationSetUp_InvokingWebServiceTwiceAndExpectationMetOnce_MessageIsSentToQueue()
        {
            MsmqHelpers.Purge("shippingservice");

            var service = Configure.Stub().NServiceBusSerializers().WcfEndPoints().Create(@".\Private$\orderservice");

            var proxy = service.EndPoint<IOrderService>("http://localhost:9101/orderservice");

            proxy.Setup(s => s.PlaceOrder(Parameter.Equals<string>(str => str == "dope"))).Returns(() => true)
               .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");

            service.Start();

            bool firstRequestReturnValue;
            bool secondRequestReturnValue;
            using (var factory = new ChannelFactory<IOrderService>(new BasicHttpBinding(), "http://localhost:9101/orderservice"))
            {
                IOrderService channel = factory.CreateChannel();

                firstRequestReturnValue = channel.PlaceOrder("dope");
                secondRequestReturnValue = channel.PlaceOrder("bar");
            }

            do
            {
                Thread.Sleep(100);
            } while (MsmqHelpers.GetMessageCount("shippingservice") == 0);

            service.Dispose();

            Assert.That(firstRequestReturnValue, Is.True);
            Assert.That(secondRequestReturnValue, Is.False);
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(1), "shipping service did not recieve send");
        }

        [Test]
        public void SimpleExpectationSetUp_BindingInputToReturnValue_InvocationValuesArePassedToReturn()
        {
            var service = Configure.Stub().NServiceBusSerializers().WcfEndPoints().Create(@".\Private$\orderservice");

            var proxy = service.EndPoint<ISomeService>("http://localhost:9101/something");

            proxy.Setup(s => s.PingBack(Parameter.Any<string>())).Returns<string>(str => str);

            service.Start();

            string firstRequestReturnValue;
            using (var factory = new ChannelFactory<ISomeService>(new BasicHttpBinding(), "http://localhost:9101/something"))
            {
                ISomeService channel = factory.CreateChannel();

                firstRequestReturnValue = channel.PingBack("hello");
            }

            service.Dispose();

            Assert.That(firstRequestReturnValue, Is.EqualTo("hello"));
        }

        [Test]
        public void SimpleExpectationSetUp_BindingInputWithMultipleParametersToReturnValue_InvocationValuesArePassedToReturn()
        {
            var service = Configure.Stub().NServiceBusSerializers().WcfEndPoints().Create(@".\Private$\orderservice");

            var proxy = service.EndPoint<ISomeService>("http://localhost:9101/something");

            proxy.Setup(s => s.IHaveMultipleInputParameters(Parameter.Any<string>(),
                Parameter.Equals<string>(str => str == "snappy"), Parameter.Any<bool>())).Returns<string, string, bool>((param1, param2, param3) => param1);

            service.Start();

            string firstRequestReturnValue;
            using (var factory = new ChannelFactory<ISomeService>(new BasicHttpBinding(), "http://localhost:9101/something"))
            {
                ISomeService channel = factory.CreateChannel();

                firstRequestReturnValue = channel.IHaveMultipleInputParameters("hello", "snappy", false);
            }

            service.Dispose();

            Assert.That(firstRequestReturnValue, Is.EqualTo("hello"));
        }
    }


}