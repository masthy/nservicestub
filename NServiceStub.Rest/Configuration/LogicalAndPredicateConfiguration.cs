using System.Collections.Generic;
using System.Linq;

namespace NServiceStub.Rest.Configuration
{
    public class LogicalAndPredicateConfiguration : IGetOrPostInvocationConfiguration
    {
        readonly IList<IGetOrPostInvocationConfiguration> _predicates = new List<IGetOrPostInvocationConfiguration>();

        public LogicalAndPredicateConfiguration(IGetOrPostInvocationConfiguration left, IGetOrPostInvocationConfiguration right)
        {
            _predicates.Add(left);
            _predicates.Add(right);
        }

        IInvocationMatcher IGetInvocationConfiguration.CreateInvocationInspector(IRouteTemplate routeToConfigure)
        {
            return new LogicalAndOfInvocations(_predicates.Select(predicate => predicate.AsGetConfiguration().CreateInvocationInspector(routeToConfigure)));
        }

        public void Add(IGetOrPostInvocationConfiguration inspection)
        {
            _predicates.Add(inspection);
        }

        IInvocationMatcher IPostInvocationConfiguration.CreateInvocationInspector(IRouteTemplate routeToConfigure)
        {
            return new LogicalAndOfInvocations(_predicates.Select(predicate => predicate.AsPostConfiguration().CreateInvocationInspector(routeToConfigure)));
        }
    }
}