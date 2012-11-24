using System;

namespace NServiceStub
{
    public interface IMessageStuffer
    {
        void PutMessageOnQueue<T>(Action<T> messageInitializer, string destinationQueue);
        void PutMessageOnQueue(object msg, string destinationQueue);
    }
}