using System.Net;

namespace NServiceStub.Rest
{
    public interface IRouteDefinition<R> : IRouteDefinition
    {
        void AddReturn(IInvocationMatcher invocation, IInvocationReturnValueProducer<R> returnValue);
    }

    public interface IRouteDefinition
    {
        bool TryInvocation(HttpListenerRequest request, out object returnValue);

        Route Route { get; }        
    }
}