using System;

namespace NServiceStub.Rest
{
    public class BodyAsDynamicEqualsPredicate : IInvocationMatcher
    {
        private readonly Func<object, bool> _bodyEvaluator;

        public BodyAsDynamicEqualsPredicate(Func<object, bool> bodyEvaluator)
        {
            _bodyEvaluator = bodyEvaluator;
        }

        public bool Matches(RequestWrapper request)
        {
            return _bodyEvaluator(request.NegotiateAndDeserializeMethodBody());
        }
    }
}