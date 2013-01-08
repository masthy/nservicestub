using System.Net;

namespace NServiceStub.Rest
{
    public interface IInvocationReturnValueProducer<R>
    {
        R Produce(HttpListenerRequest request, IRouteDefinition route);
    }
}