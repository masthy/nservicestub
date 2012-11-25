using System.Threading;
using System.Threading.Tasks;
using NServiceStub.NServiceBus;
using NUnit.Framework;
using OrderService.Contracts;

namespace NServiceStub.IntegrationTests
{
    [TestFixture]
    public class MessagePickerTests
    {
        [Test]
        public void Pick_MessageOnQueue_MessageIsDeserialized()
        {
            // Arrange
            object[] message;

            using (var bus = InternalBusCreator.CreateBus())
            {
                var picker = new MessagePicker(bus);
                var stuffer = new MessageStuffer(bus);

                stuffer.PutMessageOnQueue<IPlaceAnOrder>(msg => { msg.Product = "a"; }, "orderservice");

                // Act
                message = picker.PickMessage(@".\Private$\orderservice");
            }


            // Assert
            Assert.That(message.Length, Is.EqualTo(1));
        }

        [Test]
        public void Pick_NoMessageOnQueue_Waits()
        {
            // Arrange
            var bus = InternalBusCreator.CreateBus();
            var picker = new MessagePicker(bus);

            // Act
            Task readMessage = new Task(obj => ((MessagePicker)obj).PickMessage(@".\Private$\orderservice"), picker);

            readMessage.Start();

            Thread.Sleep(1000);

            // Assert
            bool running = !readMessage.IsCompleted && readMessage.Exception == null;

            MsmqHelpers.PutMessageOnQueue("whatever", "orderservice");
            while (!readMessage.IsCompleted) {}

            Assert.That(running);
        }

    }
}