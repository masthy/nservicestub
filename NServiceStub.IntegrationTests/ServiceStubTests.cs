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
        public void Begin_ExpectationsAreMet_SendsMessages()
        {
            // Arrange
            MsmqHelpers.Purge("orderservice");
            MsmqHelpers.Purge("shippingservice");
            MsmqHelpers.Purge("testclient");

            var bus = InternalBusCreator.CreateBus();
            var stuffer = new MessageStuffer(bus);
            var service = Configure.Stub().NServiceBusSerializers().Create(@".\Private$\orderservice");

            service.Configure()
                .Expect<IPlaceAnOrder>(msg => msg.Product == "stockings")
                .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice")
                .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "testclient");

            stuffer.PutMessageOnQueue<IPlaceAnOrder>(msg => { msg.Product = "stockings"; }, @"orderservice");

            // Act
            service.Begin();

            // Assert
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(1));
            Assert.That(MsmqHelpers.GetMessageCount("testclient"), Is.EqualTo(1));
        }

        [Test]
        public void Begin_FirstMessageOnQueueDoesNotMeetExpectations_DropsUninterestingMessageAndMovesOn()
        {
            // Arrange
            MsmqHelpers.Purge("orderservice");
            MsmqHelpers.Purge("shippingservice");

            var bus = InternalBusCreator.CreateBus();
            var stuffer = new MessageStuffer(bus);
            var service = Configure.Stub().NServiceBusSerializers().Create(@".\Private$\orderservice");

            service.Configure()
                .Expect<IPlaceAnOrder>(msg => msg.Product == "stockings")
                .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");

            stuffer.PutMessageOnQueue<IPlaceAnOrder>(msg => { msg.Product = "somethingelse"; }, @"orderservice");
            stuffer.PutMessageOnQueue<IPlaceAnOrder>(msg => { msg.Product = "stockings"; }, @"orderservice");

            // Act
            service.Begin();

            // Assert
            Assert.That(MsmqHelpers.GetMessageCount("orderservice"), Is.EqualTo(0), "meesage which did not meet the expectations was left behind");
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(1), "shipping service did not recieve send");
        }

        [Test]
        public void Begin_JustASimpleSend_SendsMessage()
        {
            // Arrange
            MsmqHelpers.Purge("shippingservice");

            var service = Configure.Stub().NServiceBusSerializers().Create(@".\Private$\orderservice");

            service.Configure()
               .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");

            // Act
            service.Begin();

            // Assert
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(1), "shipping service did not recieve send");
        }

        [Test]
        public void Begin_SendSameMessageMultipleTimes_SendsMessages()
        {
            // Arrange
            MsmqHelpers.Purge("shippingservice");

            var service = Configure.Stub().NServiceBusSerializers().Create(@".\Private$\orderservice");

            service.Configure()
               .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice").NumberOfTimes(10);

            // Act
            service.Begin();

            // Assert
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(10), "shipping service did not recieve send");
        }

        [Test]
        public void Begin_MultipleSequences_SendsMessages()
        {
            // Arrange
            MsmqHelpers.Purge("shippingservice");

            var service = Configure.Stub().NServiceBusSerializers().Create(@".\Private$\orderservice");

            service.Configure()
               .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");
            service.Configure()
               .Send<IOrderWasPlaced>(msg => msg.OrderedProduct = "stockings", "shippingservice");

            // Act
            service.Begin();

            // Assert
            Assert.That(MsmqHelpers.GetMessageCount("shippingservice"), Is.EqualTo(2), "shipping service did not recieve send");
        }

    }
}