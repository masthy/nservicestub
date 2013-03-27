using System.Linq;
using Castle.Core;
using Castle.Facilities.TypedFactory.Internal;
using Castle.MicroKernel;
using Castle.Windsor.Diagnostics;
using NServiceBus.Unicast;
using NServiceStub.Configuration;
using NServiceStub.NServiceBus;
using NServiceStub.Rest.Configuration;
using NServiceStub.WCF.Configuration;
using NUnit.Framework;
using OrderService.Contracts;

namespace NServiceStub.IntegrationTests
{
    [TestFixture]
    public class ServiceStubTests
    {
        [Test]
        public void Start_ExpectationsAreMetTwice_SendsMessagesTwice()
        {
            // Arrange
            MsmqHelpers.Purge("orderservice");
            MsmqHelpers.Purge("shippingservice");
            MsmqHelpers.Purge("testclient");

            UnicastBus bus = InternalBusCreator.CreateBus();
            var stuffer = new MessageStuffer(bus);
            ServiceStub service = Configure.Stub().NServiceBusSerializers().Create(@".\Private$\orderservice");

            service.Setup()
                   .Expect<IPlaceAnOrder>(msg => msg.Product == "stockings")
                   .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice")
                   .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "testclient");

            stuffer.PutMessageOnQueue<IPlaceAnOrder>(msg => { msg.Product = "stockings"; }, @"orderservice");
            stuffer.PutMessageOnQueue<IPlaceAnOrder>(msg => { msg.Product = "stockings"; }, @"orderservice");

            // Act
            service.Start();

            while (MsmqHelpers.GetMessageCount("shippingservice") < 2) { }
            while (MsmqHelpers.GetMessageCount("testclient") < 2) { }
            StopService(service);

            // Assert
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(2));
            Assert.That(MsmqHelpers.GetMessageCount("testclient"), Is.EqualTo(2));
        }

        [Test]
        public void Start_ExpectationsAreMet_SendsMessages()
        {
            // Arrange
            MsmqHelpers.Purge("orderservice");
            MsmqHelpers.Purge("shippingservice");
            MsmqHelpers.Purge("testclient");

            UnicastBus bus = InternalBusCreator.CreateBus();
            var stuffer = new MessageStuffer(bus);
            ServiceStub service = Configure.Stub().NServiceBusSerializers().Create(@".\Private$\orderservice");

            service.Setup()
                   .Expect<IPlaceAnOrder>(msg => msg.Product == "stockings")
                   .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice")
                   .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "testclient");

            stuffer.PutMessageOnQueue<IPlaceAnOrder>(msg => { msg.Product = "stockings"; }, @"orderservice");

            // Act
            service.Start();

            while (MsmqHelpers.GetMessageCount("shippingservice") == 0) {}
            StopService(service);

            // Assert
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(1));
            Assert.That(MsmqHelpers.GetMessageCount("testclient"), Is.EqualTo(1));
        }

        [Test]
        public void Start_FirstMessageOnQueueDoesNotMeetExpectations_DropsUninterestingMessageAndMovesOn()
        {
            // Arrange
            MsmqHelpers.Purge("orderservice");
            MsmqHelpers.Purge("shippingservice");

            UnicastBus bus = InternalBusCreator.CreateBus();
            var stuffer = new MessageStuffer(bus);
            ServiceStub service = Configure.Stub().NServiceBusSerializers().Create(@".\Private$\orderservice");

            service.Setup()
                   .Expect<IPlaceAnOrder>(msg => msg.Product == "stockings")
                   .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");

            stuffer.PutMessageOnQueue<IPlaceAnOrder>(msg => { msg.Product = "somethingelse"; }, @"orderservice");
            stuffer.PutMessageOnQueue<IPlaceAnOrder>(msg => { msg.Product = "stockings"; }, @"orderservice");

            // Act
            service.Start();
            while (MsmqHelpers.GetMessageCount("shippingservice") == 0) { }
            StopService(service);


            // Assert
            Assert.That(MsmqHelpers.GetMessageCount("orderservice"), Is.EqualTo(0), "meesage which did not meet the expectations was left behind");
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(1), "shipping service did not recieve send");
        }

        [Test]
        public void Start_JustASimpleSend_SendsMessage()
        {
            // Arrange
            MsmqHelpers.Purge("shippingservice");

            ServiceStub service = Configure.Stub().NServiceBusSerializers().Create(@".\Private$\orderservice");

            service.Setup()
                   .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");

            // Act
            service.Start();
            StopService(service);

            // Assert
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(1), "shipping service did not recieve send");
        }

        [Test]
        public void Start_MultipleSequences_SendsMessages()
        {
            // Arrange
            MsmqHelpers.Purge("shippingservice");

            ServiceStub service = Configure.Stub().NServiceBusSerializers().Create(@".\Private$\orderservice");

            service.Setup()
                   .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");
            service.Setup()
                   .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");

            // Act
            service.Start();
            StopService(service);

            // Assert
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(2), "shipping service did not recieve send");
        }

        [Test]
        public void Stop_MultipleSequences_WaitsUntilMessagesAreSent()
        {
            // Arrange
            MsmqHelpers.Purge("shippingservice");

            ServiceStub service = Configure.Stub().NServiceBusSerializers().Create(@".\Private$\orderservice");

            service.Setup()
                   .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");
            service.Setup()
                   .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");

            // Act
            service.Start();
            service.Stop();

            // Assert
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(2), "shipping service did not recieve send");
        }

        [Test]
        public void Start_SendSameMessageMultipleTimes_SendsMessages()
        {
            // Arrange
            MsmqHelpers.Purge("shippingservice");

            ServiceStub service = Configure.Stub().NServiceBusSerializers().Create(@".\Private$\orderservice");

            service.Setup()
                   .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice").NumberOfTimes(10);

            // Act
            service.Start();
            StopService(service);

            // Assert
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(10), "shipping service did not recieve send");
        }

        [Test]
        public void Start_SendSequeceOfMessagesMultipleTimes_SendsMessages()
        {
            // Arrange
            MsmqHelpers.Purge("shippingservice");

            ServiceStub service = Configure.Stub().NServiceBusSerializers().Create(@".\Private$\orderservice");

            service.Setup()
                   .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice").NumberOfTimes(10)
                   .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice").NumberOfTimes(10);
            // Act
            service.Start();
            StopService(service);

            // Assert
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(20), "shipping service did not recieve send");
        }

        [Test]
        public void Dispose_DisposingWithNServiceBusSerializer_NoTransientsLyingAround()
        {
            // Arrange
            StubConfiguration configuration = Configure.Stub();
            ServiceStub service = configuration.NServiceBusSerializers().Create(@".\Private$\orderservice");
            
            // Act
            service.Dispose();

            // Assert
            AssertThatNoTransientsAreLyingAround(configuration);
        }

        [Test]
        public void Dispose_DisposingWithNServiceBusSerializerAndWcfEndPoint_NoTransientsLyingAround()
        {
            // Arrange
            StubConfiguration configuration = Configure.Stub();
            ServiceStub service = configuration.NServiceBusSerializers().WcfEndPoints().Create(@".\Private$\orderservice");

            service.WcfEndPoint<IOrderService>("http://localhost:9202/orderservice");

            // Act
            service.Dispose();

            // Assert
            AssertThatNoTransientsAreLyingAround(configuration);
        }

        [Test]
        public void Dispose_DisposingWithNServiceBusSerializerAndRestEndPoint_NoTransientsLyingAround()
        {
            // Arrange
            StubConfiguration configuration = Configure.Stub();
            ServiceStub service = configuration.NServiceBusSerializers().Restful().Create(@".\Private$\orderservice");

            service.RestEndpoint("http://localhost:9202/orderservice/");

            // Act
            service.Dispose();

            // Assert
            AssertThatNoTransientsAreLyingAround(configuration);
        }

        private static void AssertThatNoTransientsAreLyingAround(StubConfiguration configuration)
        {
            var diagnostics = configuration.Container.Kernel.GetSubSystem(SubSystemConstants.DiagnosticsKey) as IDiagnosticsHost;
            var diagnostic = diagnostics.GetDiagnostic<ITrackedComponentsDiagnostic>();

            ILookup<IHandler, object> handlers = diagnostic.Inspect();

            foreach (var handler in handlers)
            {
                if (handler.Key.ComponentModel.LifestyleType != LifestyleType.Singleton &&
                    !typeof(TypedFactoryInterceptor).IsAssignableFrom(handler.Key.ComponentModel.Implementation))
                {
                    Assert.Fail("Component {0} is still hanging in there after dispose", handler.Key.ComponentModel.Implementation);
                }
            }
        }

        private static void StopService(ServiceStub service)
        {
            service.Stop();
            while (service.IsRunning)
            {}

            service.Dispose();
        }
    }
}