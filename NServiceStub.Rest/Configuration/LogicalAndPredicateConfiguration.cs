using System.Collections.Generic;
using System.Linq;

namespace NServiceStub.Rest.Configuration
{
    public class LogicalAndPredicateConfiguration : IRouteInvocationConfiguration
    {
        readonly IList<IRouteInvocationConfiguration> _predicates = new List<IRouteInvocationConfiguration>();

        public LogicalAndPredicateConfiguration(IRouteInvocationConfiguration left, IRouteInvocationConfiguration right)
        {
            _predicates.Add(left);
            _predicates.Add(right);
        }

        public IInvocationMatcher CreateInvocationInspector()
        {
            return new LogicalAndOfInvocations(_predicates.Select(predicate => predicate.CreateInvocationInspector()));
        }

        public void Add(IRouteInvocationConfiguration inspection)
        {
            _predicates.Add(inspection);
        }
    }
}