using System;
using System.IO;
using NServiceBus;
using NServiceBus.MessageInterfaces;
using NServiceBus.Serialization;
using NServiceBus.Transports;
using NServiceBus.Unicast;

namespace NServiceStub.NServiceBus
{
    public class MessageStuffer : IMessageStuffer
    {
        private readonly IMessageSerializer _messageSerializer;
        private readonly ISendMessages _messageSender;
        private readonly IMessageMapper _messageMapper;

        public MessageStuffer(UnicastBus bus)
        {
            _messageSerializer = bus.Builder.Build<IMessageSerializer>();
            _messageSender = bus.Builder.Build<ISendMessages>();
            _messageMapper = bus.Builder.Build<IMessageMapper>();
        }

        public void PutMessageOnQueue<T>(Action<T> messageInitializer, string destinationQueue)
        {
            PutMessageOnQueue(_messageMapper.CreateInstance(messageInitializer), destinationQueue);
        }

        public void PutMessageOnQueue(object msg, string destinationQueue)
        {
            Address address = Address.Parse(destinationQueue);
            var transportMessage = new TransportMessage
                {
                    CorrelationId = null,
                    MessageIntent = MessageIntentEnum.Send
                };
            MapTransportMessageFor(msg, transportMessage);
            
            _messageSender.Send(transportMessage, new SendOptions(address));
        }

        private void MapTransportMessageFor(object message, TransportMessage result)
        {
            var memoryStream = new MemoryStream();
            _messageSerializer.Serialize(message, memoryStream);
            result.Body = memoryStream.ToArray();
            result.TimeToBeReceived = TimeSpan.MaxValue;
        }

    }
}