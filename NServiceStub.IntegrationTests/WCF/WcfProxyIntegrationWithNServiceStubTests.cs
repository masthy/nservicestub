using System.ServiceModel;
using System.Threading;
using Moq;
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
        public void TcpBinding_UsingTcpBindingToConnect_CanConnectSuccessfully()
        {
            var service = Configure.Stub().NServiceBusSerializers().WcfEndPoints().Create(@".\Private$\orderservice");

            service.WcfEndPoint<IOrderService>("net.tcp://localhost:9101/orderservice");

            service.Start();

            using (var factory = new ChannelFactory<IOrderService>(new NetTcpBinding(), "net.tcp://localhost:9101/orderservice"))
            {
                IOrderService channel = factory.CreateChannel();

                channel.PlaceOrder("bar");
            }

            service.Dispose();
        }

        [Test]
        public void NoBindingSpecified_UsingBindingInConfigurationFile_CanConnectSuccessfully()
        {
            var service = Configure.Stub().NServiceBusSerializers().WcfEndPoints().Create(@".\Private$\orderservice");

            service.WcfEndPoint<ISomeServiceConfiguredInAppConfig>();

            service.Start();

            var netTcpBinding = new NetTcpBinding {Security = {Mode = SecurityMode.None}};
            using (var factory = new ChannelFactory<ISomeServiceConfiguredInAppConfig>(netTcpBinding, "net.tcp://localhost:9102/someservice"))
            {
                ISomeServiceConfiguredInAppConfig channel = factory.CreateChannel();

                channel.Hello();
            }

            service.Dispose();
        }

        [Test]
        public void Fallback_IAlreadyHaveAServiceImplementation_CallingImplementation()
        {
            MsmqHelpers.Purge("shippingservice");

            var service = Configure.Stub().NServiceBusSerializers().WcfEndPoints().Create(@".\Private$\orderservice");

            var existingImpl = new Mock<ISomeService>();
            service.WcfEndPoint("http://localhost:9101/boo", existingImpl.Object);

            service.Start();

            using (var factory = new ChannelFactory<ISomeService>(new BasicHttpBinding(), "http://localhost:9101/boo"))
            {
                ISomeService channel = factory.CreateChannel();

                channel.AVoidServiceMethod();
            }

            service.Dispose();

            existingImpl.Verify(m => m.AVoidServiceMethod());
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
        public void VoidServiceMethod_InvokeAndSend_MessageIsSent()
        {
            MsmqHelpers.Purge("shippingservice");
            var service = Configure.Stub().NServiceBusSerializers().WcfEndPoints().Create(@".\Private$\orderservice");

            var proxy = service.WcfEndPoint<ISomeService>("http://localhost:9101/something");

            proxy.Setup(s => s.AVoidServiceMethod()).Send<IOrderWasPlaced>(msg => { msg.OrderedProduct = "abbazz"; }, "shippingservice");

            service.Start();

            using (var factory = new ChannelFactory<ISomeService>(new BasicHttpBinding(), "http://localhost:9101/something"))
            {
                ISomeService channel = factory.CreateChannel();

                channel.AVoidServiceMethod();
            }

            service.Dispose();

            Assert.That(MsmqHelpers.PickMessageBody("shippingservice"), Contains.Substring("abbazz"));
        }

        [Test]
        public void SimpleExpectationSetUp_BindingInputToReturnValueByName_InvocationValuesArePassedToReturn()
        {
            var service = Configure.Stub().NServiceBusSerializers().WcfEndPoints().Create(@".\Private$\orderservice");

            var proxy = service.WcfEndPoint<ISomeService>("http://localhost:9101/something");

            proxy.Setup(s => s.IHave4InputParameters(Parameter.Any<string>(), Parameter.Any<string>(), Parameter.Any<bool>(), Parameter.Any<string>())).Returns<string>(fallback => new FourInputParamsReturnValue{ ReturnOne = fallback });

            service.Start();

            FourInputParamsReturnValue actual;
            using (var factory = new ChannelFactory<ISomeService>(new BasicHttpBinding(), "http://localhost:9101/something"))
            {
                ISomeService channel = factory.CreateChannel();

                actual = channel.IHave4InputParameters("john doe", "somewhere", false, "to this");
            }

            service.Dispose();

            Assert.That(actual.ReturnOne, Is.EqualTo("to this"));
        }

        [Test]
        public void SimpleExpectationSetUp_BindingInputWithMultipleParametersToReturnValue_InvocationValuesArePassedToReturn()
        {
            var service = Configure.Stub().NServiceBusSerializers().WcfEndPoints().Create(@".\Private$\orderservice");

            var proxy = service.WcfEndPoint<ISomeService>("http://localhost:9101/something");

            proxy.Setup(s => s.IHaveMultipleInputParameters(Parameter.Any<string>(), Parameter.Equals<string>(str => str == "snappy"), Parameter.Any<bool>())).Returns<string, string, bool>((param1, param2, param3) => param1);

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
        public void SimpleExpectationSetUp_BindingInputWithMultipleParametersToReturnValue2_InvocationValuesArePassedToReturn()
        {
            var service = Configure.Stub().NServiceBusSerializers().WcfEndPoints().Create(@".\Private$\orderservice");

            var proxy = service.WcfEndPoint<ISomeService>("http://localhost:9101/something");

            proxy.Setup(s => s.IHave4InputParameters(Parameter.Any<string>(), Parameter.Equals<string>(str => str == "snappy"), Parameter.Any<bool>(), Parameter.Any<string>())).Returns<string, string, bool, string>((param1, param2, param3, param4) => 
                new FourInputParamsReturnValue{ReturnOne = param1, ReturnTwo = param2, ReturnThree = param3, ReturnFour = param4});

            service.Start();

            FourInputParamsReturnValue retVal;
            using (var factory = new ChannelFactory<ISomeService>(new BasicHttpBinding(), "http://localhost:9101/something"))
            {
                ISomeService channel = factory.CreateChannel();

                retVal = channel.IHave4InputParameters("hello", "snappy", false, "poppy");
            }

            service.Dispose();

            Assert.That(retVal.ReturnOne, Is.EqualTo("hello"));
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
        public void SimpleExpectationSetUp_BindingInputWithMultipleParametersToReturnValue_InvocationValuesAreAccessibleWhenStubbingMessage()
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

        [Test]
        public void SimpleExpectationSetUp_BindingInputWithMultipleParametersToReturnValue2_InvocationValuesAreAccessibleWhenStubbingMessage()
        {
            MsmqHelpers.Purge("shippingservice");
            var service = Configure.Stub().NServiceBusSerializers().WcfEndPoints().Create(@".\Private$\orderservice");

            var proxy = service.WcfEndPoint<ISomeService>("http://localhost:9101/something");

            proxy.Setup(s => s.IHave4InputParameters(Parameter.Any<string>(), Parameter.Equals<string>(str => str == "snappy"), Parameter.Any<bool>(), Parameter.Any<string>())).Returns(() => new FourInputParamsReturnValue())
                .Send<IOrderWasPlaced, string, string, bool, string>((msg, param1, param2, param3, param4) => { msg.OrderedProduct = param4; }, "shippingservice");

            service.Start();

            using (var factory = new ChannelFactory<ISomeService>(new BasicHttpBinding(), "http://localhost:9101/something"))
            {
                ISomeService channel = factory.CreateChannel();

                channel.IHave4InputParameters("hello", "snappy", false, "bar");
            }

            service.Dispose();

            Assert.That(MsmqHelpers.PickMessageBody("shippingservice"), Is.StringContaining("bar"));
        }

        [Test]
        public void SpecifyWcfEndpoint_FetchingSameEndpointTwice_GetsSameInstanceBack()
        {
            var service = Configure.Stub().NServiceBusSerializers().WcfEndPoints().Create(@".\Private$\orderservice");

            var proxy1 = service.WcfEndPoint<IOrderService>("http://localhost:9101/orderservice");
            var proxy2 = service.WcfEndPoint<IOrderService>("http://localhost:9101/orderservice");

            service.Dispose();

            Assert.That(proxy1, Is.SameAs(proxy2));
        }
    }


}