using System.Threading;
using System.Threading.Tasks;
using NServiceStub.NServiceBus;
using NUnit.Framework;
using OrderService.Contracts;

namespace NServiceStub.IntegrationTests.NServiceBus
{
    [TestFixture]
    public class MessagePickerTests
    {
        [Test]
        public void Pick_MessageOnQueue_MessageIsDeserialized()
        {
            // Arrange
            object[] message;

            var bus = InternalBusCreator.CreateBus();

            var picker = new MessagePicker(bus);
            var stuffer = new MessageStuffer(bus);

            stuffer.PutMessageOnQueue<IPlaceAnOrder>(msg => { msg.Product = "a"; }, "orderservice");

            // Act
            message = picker.PickMessage(@".\Private$\orderservice");

            // Assert
            Assert.That(message.Length, Is.EqualTo(1));
        }

        [Test]
        public void Pick_NoMessageOnQueue_ReturnsNull()
        {
            // Arrange
            var bus = InternalBusCreator.CreateBus();
            var picker = new MessagePicker(bus);

            // Act
            object[] nextMessage = picker.PickMessage(@".\Private$\orderservice");

            // Assert
            Assert.That(nextMessage, Is.Null);
        }

    }
}