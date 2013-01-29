using System.Collections.Generic;
using System.Net;

namespace NServiceStub.Rest
{
    public class PostTemplate : IPostTemplate
    {
        readonly Dictionary<IInvocationMatcher, IInvocationReturnValueProducer<object>> _invocationVsReturnValue = new Dictionary<IInvocationMatcher, IInvocationReturnValueProducer<object>>();

        public PostTemplate(Post route)
        {
            Route = route;
        }

        public void AddReturn(IInvocationMatcher invocation, IInvocationReturnValueProducer<object> returnValue)
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

        public Post Route { get; private set; }
    }
}