using System.Net;

namespace NServiceStub.Rest
{
    public interface IInvocationReturnValueProducer
    {
        object Produce(HttpListenerRequest request);
    }
}