using System;

namespace NServiceStub.Rest.Configuration
{
    public class BodyAsDynamicEqualsConfiguration : IPostInvocationConfiguration
    {
        private readonly Func<dynamic, bool> _bodyEvaluator;

        public BodyAsDynamicEqualsConfiguration(Func<dynamic, bool> bodyEvaluator)
        {
            _bodyEvaluator = bodyEvaluator;
        }

        public IInvocationMatcher CreateInvocationInspector(IPostTemplate routeToConfigure)
        {
            return new BodyAsDynamicEqualsPredicate(_bodyEvaluator);
        }
    }
}