using System.Messaging;
using NServiceBus.Unicast;

namespace NServiceStub.NServiceBus
{
    public class MessagePicker : IMessagePicker
    {
        private readonly UnicastBus _bus;

        public MessagePicker(UnicastBus bus)
        {
            _bus = bus;
        }

        public object[] PickMessage(string fromQueue)
        {
            using (var queue = new MessageQueue(fromQueue))
            {
                using (MessageEnumerator messageEnumerator2 = queue.GetMessageEnumerator2())
                {
                    if (!messageEnumerator2.MoveNext())
                        return null;

                    Message current = messageEnumerator2.Current;
                    messageEnumerator2.RemoveCurrent();

                    return DeserializeMessage(current);
                }
            }
        }

        private object[] DeserializeMessage(Message message)
        {
            return _bus.MessageSerializer.Deserialize(message.BodyStream);
        }
    }
}