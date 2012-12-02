using NServiceBus.Unicast;
using NServiceStub.Configuration;
using NServiceStub.NServiceBus;
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

            service.Configure()
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

            service.Configure()
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

            service.Configure()
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

            service.Configure()
                   .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");

            // Act
            service.Start();
            while (MsmqHelpers.GetMessageCount("shippingservice") == 0) { }
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

            service.Configure()
                   .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");
            service.Configure()
                   .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");

            // Act
            service.Start();
            while (MsmqHelpers.GetMessageCount("shippingservice") < 2) { }
            StopService(service);

            // Assert
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(2), "shipping service did not recieve send");
        }

        [Test]
        public void Start_SendSameMessageMultipleTimes_SendsMessages()
        {
            // Arrange
            MsmqHelpers.Purge("shippingservice");

            ServiceStub service = Configure.Stub().NServiceBusSerializers().Create(@".\Private$\orderservice");

            service.Configure()
                   .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice").NumberOfTimes(10);

            // Act
            service.Start();
            while (MsmqHelpers.GetMessageCount("shippingservice") < 10) { }
            StopService(service);

            // Assert
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(10), "shipping service did not recieve send");
        }

        private static void StopService(ServiceStub service)
        {
            service.RequestStop();
            while (service.IsRunning)
            {
            }
        }
    }
}