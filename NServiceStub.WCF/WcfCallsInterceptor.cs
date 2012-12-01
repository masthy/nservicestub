using System;
using System.Collections.Generic;
using Castle.DynamicProxy;

namespace NServiceStub.WCF
{
    public class WcfCallsInterceptor : IInterceptor
    {
        readonly Dictionary<IInvocationMatcher, Func<object>> _invocationVersusReturnValue = new Dictionary<IInvocationMatcher, Func<object>>();

        public void Intercept(IInvocation invocation)
        {
            foreach (var matchVsReturnValue in _invocationVersusReturnValue)
            {
                if (matchVsReturnValue.Key.Matches(invocation.Arguments))
                {
                    invocation.ReturnValue = matchVsReturnValue.Value();
                    return;
                }
            }

            if (invocation.Method.ReturnType != typeof(void))
            {
                if (invocation.Method.ReturnType.IsValueType)
                    invocation.ReturnValue = Activator.CreateInstance(invocation.Method.ReturnType);
                else
                    invocation.ReturnValue = null;                    
            }
        }

        public void AddInvocation(IInvocationMatcher matcher, Func<object> returnValueProducer)
        {
            _invocationVersusReturnValue.Add(matcher, returnValueProducer);            
        }
    }
}