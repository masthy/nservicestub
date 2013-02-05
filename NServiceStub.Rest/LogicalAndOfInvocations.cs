using System.Collections.Generic;
using System.Linq;

namespace NServiceStub.Rest
{
    public class LogicalAndOfInvocations : IInvocationMatcher
    {
        private readonly IEnumerable<IInvocationMatcher> _predicatesToCombine;

        public LogicalAndOfInvocations(IEnumerable<IInvocationMatcher> predicatesToCombine)
        {
            _predicatesToCombine = predicatesToCombine;
        }

        public bool Matches(RequestWrapper request)
        {
            return _predicatesToCombine.All(predicate => predicate.Matches(request));
        }

    }
}