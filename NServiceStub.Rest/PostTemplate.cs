//using System.Collections.Generic;

//namespace NServiceStub.Rest
//{
//    public class PostTemplate : IRouteTemplate
//    {
//        readonly Dictionary<IInvocationMatcher, IInvocationReturnValueProducer<object>> _invocationVsReturnValue = new Dictionary<IInvocationMatcher, IInvocationReturnValueProducer<object>>();

//        public PostTemplate(Post route)
//        {
//            Route = route;
//        }

//        public void AddReturn(IInvocationMatcher invocation, IInvocationReturnValueProducer<object> returnValue)
//        {
//            _invocationVsReturnValue.Add(invocation, returnValue);
//        }

//        public bool TryInvocation(RequestWrapper request, out object returnValue)
//        {
//            foreach (var invationVsReturnValue in _invocationVsReturnValue)
//            {
//                if (invationVsReturnValue.Key.Matches(request))
//                {
//                    returnValue = invationVsReturnValue.Value.Produce(request.Request);
//                    return true;
//                }
//            }

//            returnValue = null;
//            return false;
//        }

//        public bool Matches(RequestWrapper request)
//        {
//            return Route.Matches(request.Request.RawUrl);
//        }

//        public Post Route { get; private set; }
//    }
//}