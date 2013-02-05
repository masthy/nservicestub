using System.Collections.Generic;

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

        public Get Route { get; private set; }
    }
}