using System.Collections.Generic;
using System.Net;

namespace NServiceStub.Rest
{
    public class RouteDefinition<R> : IRouteDefinition<R>
    {
        readonly Dictionary<IInvocationMatcher, IInvocationReturnValueProducer<R>> _invocationVsReturnValue = new Dictionary<IInvocationMatcher, IInvocationReturnValueProducer<R>>();

        public RouteDefinition(Route route)
        {
            Route = route;
        }

        public void AddReturn(IInvocationMatcher invocation, IInvocationReturnValueProducer<R> returnValue)
        {
            _invocationVsReturnValue.Add(invocation, returnValue);
        }

        public bool TryInvocation(HttpListenerRequest request, out object returnValue)
        {
            foreach (var invationVsReturnValue in _invocationVsReturnValue)
            {
                if (invationVsReturnValue.Key.Matches(request, this))
                {
                    returnValue = invationVsReturnValue.Value.Produce(request, this);
                    return true;
                }
            }

            returnValue = null;
            return false;
        }

        public Route Route { get; private set; }
    }
}