using System;
using System.Collections.Generic;
using System.IO;
using NServiceBus;
using NServiceBus.Unicast;
using NServiceBus.Unicast.Transport;

namespace NServiceStub.NServiceBus
{
    public class MessageStuffer : IMessageStuffer
    {
        private readonly UnicastBus _busUsedToSerializeMessages;

        public MessageStuffer(UnicastBus busUsedToSerializeMessages)
        {
            _busUsedToSerializeMessages = busUsedToSerializeMessages;
        }

        public void PutMessageOnQueue<T>(Action<T> messageInitializer, string destinationQueue)
        {
            PutMessageOnQueue(_busUsedToSerializeMessages.CreateInstance(messageInitializer), destinationQueue);
        }

        public void PutMessageOnQueue(object msg, string destinationQueue)
        {
            Address address = Address.Parse(destinationQueue);

            var transportMessage = new TransportMessage
                {
                    CorrelationId = null,
                    MessageIntent = MessageIntentEnum.Send
                };
            MapTransportMessageFor(_busUsedToSerializeMessages, new[] { msg }, transportMessage);
            _busUsedToSerializeMessages.MessageSender.Send(transportMessage, address);
        }

        private static void MapTransportMessageFor(UnicastBus bus, object[] rawMessages, TransportMessage result)
        {
            result.Headers = new Dictionary<string, string>();
            result.ReplyToAddress = Address.Local;
            object[] messages = rawMessages;
            var memoryStream = new MemoryStream();
            bus.MessageSerializer.Serialize(messages, memoryStream);
            result.Body = memoryStream.ToArray();
            result.ReplyToAddress = Address.Local;
            result.TimeToBeReceived = TimeSpan.MaxValue;
        }

    }
}