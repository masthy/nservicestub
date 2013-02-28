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

            var proxy = service.WcfEndPoint<IOrderService>("http://localhost:9101/orderservice");

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
        public void ExecuteServiceMethod_WhichReturnsComplexType_ReturnsType()
        {
            var service = Configure.Stub().NServiceBusSerializers().WcfEndPoints().Create(@"whatever");

            var proxy = service.WcfEndPoint<IOrderService>("http://localhost:9101/orderservice");

            proxy.Setup(s => s.PlaceOrder(Parameter.Any<string>())).Returns(() => true);
            proxy.Setup(s => s.WhenWasOrderLastPlaced()).Returns(() => new Date(2000, 1, 1));

            service.Start();

            Date returnValue;

            using (var factory = new ChannelFactory<IOrderService>(new BasicHttpBinding(), "http://localhost:9101/orderservice"))
            {
                IOrderService channel = factory.CreateChannel();

                returnValue = channel.WhenWasOrderLastPlaced();
            }

            service.Dispose();

            Assert.That(returnValue, Is.EqualTo(new Date(2000, 1, 1)));
        }

        [Test]
        public void ExecuteServiceMethod_TwoSetupsWithNoParameters_CorrectMethodIsInvoked()
        {
            var service = Configure.Stub().NServiceBusSerializers().WcfEndPoints().Create(@"whatever");

            var proxy = service.WcfEndPoint<IOrderService>("http://localhost:9101/orderservice");

            proxy.Setup(s => s.AreYouHappy()).Returns(() => true);
            proxy.Setup(s => s.WhenWasOrderLastPlaced()).Returns(() => new Date(2000, 1, 1));

            service.Start();

            Date returnValue;

            using (var factory = new ChannelFactory<IOrderService>(new BasicHttpBinding(), "http://localhost:9101/orderservice"))
            {
                IOrderService channel = factory.CreateChannel();

                returnValue = channel.WhenWasOrderLastPlaced();
            }

            service.Dispose();

            Assert.That(returnValue, Is.EqualTo(new Date(2000, 1, 1)));
        }

        [Test]
        public void SimpleExpectationSetUp_UsingServiceKnownType_HandledAndMessageIsSentToQueue()
        {
            MsmqHelpers.Purge("shippingservice");

            var service = Configure.Stub().NServiceBusSerializers().WcfEndPoints().Create(@".\Private$\orderservice");

            var proxy = service.WcfEndPoint<IOrderService>("http://localhost:9101/orderservice");

            proxy.Setup(s => s.ExecuteCommand(Parameter.Any<DeleteOrder>()))
               .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");

            service.Start();

            using (var factory = new ChannelFactory<IOrderService>(new BasicHttpBinding(), "http://localhost:9101/orderservice"))
            {
                IOrderService channel = factory.CreateChannel();

                channel.ExecuteCommand(new DeleteOrder());
            }

            MsmqHelpers.WaitForMessages("shippingservice");

            service.Dispose();

            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(1), "shipping service did not recieve send");
        }

        [Test]
        public void SimpleExpectationSetUp_UsingServiceKnownType2_HandledAndMessageIsSentToQueue()
        {
            MsmqHelpers.Purge("shippingservice");

            var service = Configure.Stub().NServiceBusSerializers().WcfEndPoints().Create(@".\Private$\orderservice");

            var proxy = service.WcfEndPoint<IOrderService>("http://localhost:9101/orderservice");

            proxy.Setup(s => s.ExecuteCommand(Parameter.Equals<DeleteOrder>(command => command.OrderNumber == 1)))
               .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");

            service.Start();

            using (var factory = new ChannelFactory<IOrderService>(new BasicHttpBinding(), "http://localhost:9101/orderservice"))
            {
                IOrderService channel = factory.CreateChannel();

                channel.ExecuteCommand(new DoSomethingWithOrder());
                channel.ExecuteCommand(new DeleteOrder());
                channel.ExecuteCommand(new DeleteOrder{OrderNumber = 1});
            }

            MsmqHelpers.WaitForMessages("shippingservice");

            service.Dispose();

            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(1), "shipping service did not recieve send");
        }

        [Test]
        public void SimpleExpectationSetUp_UsingServiceKnownTypeAndInvokedWithWrongSignature_DoesNotHandleMessage()
        {
            MsmqHelpers.Purge("shippingservice");

            var service = Configure.Stub().NServiceBusSerializers().WcfEndPoints().Create(@".\Private$\orderservice");

            var proxy = service.WcfEndPoint<IOrderService>("http://localhost:9101/orderservice");

            proxy.Setup(s => s.ExecuteCommand(Parameter.Any<DeleteOrder>()))
               .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");

            service.Start();

            using (var factory = new ChannelFactory<IOrderService>(new BasicHttpBinding(), "http://localhost:9101/orderservice"))
            {
                IOrderService channel = factory.CreateChannel();

                channel.ExecuteCommand(new DoSomethingWithOrder());
            }

            service.Dispose();

            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(0), "shipping service recieved message");
        }

        [Test]
        public void SimpleExpectationSetUp_BindingInputToReturnValue_InvocationValuesArePassedToReturn()
        {
            var service = Configure.Stub().NServiceBusSerializers().WcfEndPoints().Create(@".\Private$\orderservice");

            var proxy = service.WcfEndPoint<ISomeService>("http://localhost:9101/something");

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

            var proxy = service.WcfEndPoint<ISomeService>("http://localhost:9101/something");

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

        [Test]
        public void SimpleExpectationSetUp_BindingInputWithMultipleParametersSomeParametersOfTheReturnValue_InvocationValuesArePassedToReturn()
        {
            var service = Configure.Stub().NServiceBusSerializers().WcfEndPoints().Create(@".\Private$\orderservice");

            var proxy = service.WcfEndPoint<ISomeService>("http://localhost:9101/something");

            proxy.Setup(s => s.IHaveMultipleInputParameters(Parameter.Any<string>(),
                Parameter.Equals<string>(str => str == "snappy"), Parameter.Any<bool>())).Returns<bool>(param3 => param3.ToString());

            service.Start();

            string returnValue;
            using (var factory = new ChannelFactory<ISomeService>(new BasicHttpBinding(), "http://localhost:9101/something"))
            {
                ISomeService channel = factory.CreateChannel();

                returnValue = channel.IHaveMultipleInputParameters("hello", "snappy", false);
            }

            service.Dispose();

            Assert.That(returnValue, Is.EqualTo("False"));
        }

        [Test]
        public void SimpleExpectationSetUp_BindingInputWithMultipleParametersToReturnValue_InvocationValuesArePassedDownTheChain()
        {
            MsmqHelpers.Purge("shippingservice");
            var service = Configure.Stub().NServiceBusSerializers().WcfEndPoints().Create(@".\Private$\orderservice");

            var proxy = service.WcfEndPoint<ISomeService>("http://localhost:9101/something");

            proxy.Setup(s => s.IHaveMultipleInputParameters(Parameter.Any<string>(), Parameter.Equals<string>(str => str == "snappy"), Parameter.Any<bool>())).Returns<string, string, bool>((param1, param2, param3) => param1)
                .Send<IOrderWasPlaced, string>((msg, product) => { msg.OrderedProduct = product; }, "shippingservice");

            service.Start();

            string firstRequestReturnValue;
            using (var factory = new ChannelFactory<ISomeService>(new BasicHttpBinding(), "http://localhost:9101/something"))
            {
                ISomeService channel = factory.CreateChannel();

                firstRequestReturnValue = channel.IHaveMultipleInputParameters("hello", "snappy", false);
            }

            service.Dispose();

            Assert.That(MsmqHelpers.PickMessageBody("shippingservice"), Is.StringContaining("hello"));
            Assert.That(firstRequestReturnValue, Is.EqualTo("hello"));
        }

    }


}