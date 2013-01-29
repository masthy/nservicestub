using System.Collections.Generic;
using System.Net;

namespace NServiceStub.Rest
{
    public class GetTemplate<R> : IGetTemplate<R>
    {
        readonly Dictionary<IInvocationMatcher, IInvocationReturnValueProducer<R>> _invocationVsReturnValue = new Dictionary<IInvocationMatcher, IInvocationReturnValueProducer<R>>();

        public GetTemplate(Get route)
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
                if (invationVsReturnValue.Key.Matches(request))
                {
                    returnValue = invationVsReturnValue.Value.Produce(request);
                    return true;
                }
            }

            returnValue = null;
            return false;
        }

        public bool Matches(HttpListenerRequest request)
        {
            return Route.Matches(request.RawUrl);
        }

        public Get Route { get; private set; }
    }
}