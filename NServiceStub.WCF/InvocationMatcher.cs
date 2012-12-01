using System;
using System.Linq;

namespace NServiceStub.WCF
{
    public class InvocationMatcher : IInvocationMatcher
    {
        private readonly Func<object, bool>[] _parameterMatchers;

        public InvocationMatcher(Func<object, bool>[] parameterMatchers)
        {
            _parameterMatchers = parameterMatchers;
        }

        public bool Matches(object[] arguments)
        {
            int index = 0;
            return _parameterMatchers.All(matcher => matcher(arguments[index++]));
        }
    }
}