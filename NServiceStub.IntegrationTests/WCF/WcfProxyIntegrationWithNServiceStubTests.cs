using System.ServiceModel;
using System.Threading;
using NServiceStub.Configuration;
using NServiceStub.NServiceBus;
using NServiceStub.WCF;
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

            var proxy = new WcfProxy<IOrderService>("http://localhost:9101/orderservice");

            var service = Configure.Stub().NServiceBusSerializers().Create(@".\Private$\orderservice");

            proxy.Setup(s => s.PlaceOrder(Parameter.Equals<string>(str => str == "dope"))).Returns(() => true)
               .Send<IOrderWasPlaced>(service, msg => msg.OrderedProduct = "stockings", "shippingservice");

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

            Assert.That(firstRequestReturnValue, Is.True);
            Assert.That(secondRequestReturnValue, Is.False);
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(1), "shipping service did not recieve send");
        }
    }


}