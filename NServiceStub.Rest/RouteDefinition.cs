using System.Collections.Generic;

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

        public bool TryInvocation(string rawUrl, out object returnValue)
        {
            foreach (var invationVsReturnValue in _invocationVsReturnValue)
            {
                if (invationVsReturnValue.Key.Matches(rawUrl, this))
                {
                    returnValue = invationVsReturnValue.Value.Produce(rawUrl, this);
                    return true;
                }
            }

            returnValue = null;
            return false;
        }

        public Route Route { get; private set; }
    }
}