using System;

namespace NServiceStub
{
    public class Sender<T> : ISender
    {
        private readonly Action<T> _msgInitializer;
        private readonly IMessageStuffer _stuffer;
        private readonly string _destinationQueue;


        public Sender(IMessageStuffer stuffer, string destinationQueue, Action<T> msgInitializer)
        {
            _stuffer = stuffer;
            _destinationQueue = destinationQueue;
            _msgInitializer = msgInitializer;
        }

        public void Send()
        {
            _stuffer.PutMessageOnQueue(_msgInitializer, _destinationQueue);
        }

    }
}