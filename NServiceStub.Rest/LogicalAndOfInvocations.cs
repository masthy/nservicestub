using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace NServiceStub.Rest
{
    public class LogicalAndOfInvocations : IInvocationMatcher
    {
        private readonly IEnumerable<IInvocationMatcher> _predicatesToCombine;

        public LogicalAndOfInvocations(IEnumerable<IInvocationMatcher> predicatesToCombine)
        {
            _predicatesToCombine = predicatesToCombine;
        }

        public bool Matches(HttpListenerRequest request, IRouteDefinition routeOwningUrl)
        {
            return _predicatesToCombine.All(predicate => predicate.Matches(request, routeOwningUrl));
        }
    }
}