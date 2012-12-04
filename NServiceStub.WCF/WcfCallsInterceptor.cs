using System;
using System.Collections.Generic;
using Castle.DynamicProxy;

namespace NServiceStub.WCF
{
    public class WcfCallsInterceptor : IInterceptor
    {
        readonly Dictionary<IInvocationMatcher, IInvocationReturnValueProducer> _invocationVersusReturnValue = new Dictionary<IInvocationMatcher, IInvocationReturnValueProducer>();

        public void Intercept(IInvocation invocation)
        {
            foreach (var matchVsReturnValue in _invocationVersusReturnValue)
            {
                if (matchVsReturnValue.Key.Matches(invocation.Arguments))
                {
                    invocation.ReturnValue = matchVsReturnValue.Value.Produce(invocation.Arguments);
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

        public void AddInvocation(IInvocationMatcher matcher, IInvocationReturnValueProducer returnValueProducer)
        {
            _invocationVersusReturnValue.Add(matcher, returnValueProducer);        
        }
    }
}