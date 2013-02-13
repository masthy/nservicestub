using System;
using System.Collections.Generic;

namespace NServiceStub.Rest
{
    public class RouteTemplate<R> : IRouteTemplate<R>
    {
        readonly Dictionary<IInvocationMatcher, IInvocationReturnValueProducer> _invocationVsReturnValue = new Dictionary<IInvocationMatcher, IInvocationReturnValueProducer>();

        public RouteTemplate(Route route)
        {
            Route = route;
        }

        public void AddReturn(IInvocationMatcher invocation, IInvocationReturnValueProducer returnValue)
        {
            _invocationVsReturnValue.Add(invocation, returnValue);
        }

        public bool TryInvocation(RequestWrapper request, out object returnValue)
        {
            foreach (var invationVsReturnValue in _invocationVsReturnValue)
            {
                if (invationVsReturnValue.Key.Matches(request))
                {
                    returnValue = invationVsReturnValue.Value.Produce(request.Request);
                    return true;
                }
            }

            returnValue = null;
            return false;
        }

        public bool Matches(RequestWrapper request)
        {
            return Route.Matches(request.Request.RawUrl);
        }

        public Route Route { get; private set; }
    }
}