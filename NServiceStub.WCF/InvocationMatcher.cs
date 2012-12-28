using System;
using System.Linq;
using System.Reflection;

namespace NServiceStub.WCF
{
    public class InvocationMatcher : IInvocationMatcher
    {
        private readonly Func<object, bool>[] _parameterMatchers;
        private readonly MethodInfo _inspectedMethod;

        public InvocationMatcher(Func<object, bool>[] parameterMatchers, MethodInfo inspectedMethod)
        {
            _parameterMatchers = parameterMatchers;
            _inspectedMethod = inspectedMethod;
        }

        public bool Matches(object[] arguments)
        {
            int index = 0;
            return _parameterMatchers.All(matcher => matcher(arguments[index++]));
        }

        public MethodInfo InspectedMethod { get { return _inspectedMethod; } }
    }
}