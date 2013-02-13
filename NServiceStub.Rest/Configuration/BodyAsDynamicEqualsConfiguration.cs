using System;

namespace NServiceStub.Rest.Configuration
{
    public class BodyAsDynamicEqualsConfiguration : IGetOrPostInvocationConfiguration
    {
        private readonly Func<dynamic, bool> _bodyEvaluator;

        public BodyAsDynamicEqualsConfiguration(Func<dynamic, bool> bodyEvaluator)
        {
            _bodyEvaluator = bodyEvaluator;
        }

        public IInvocationMatcher CreateInvocationInspector(IRouteTemplate routeToConfigure)
        {
            return new BodyAsDynamicEqualsPredicate(_bodyEvaluator);
        }
    }
}