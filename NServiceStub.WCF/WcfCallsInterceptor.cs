using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.DynamicProxy;

namespace NServiceStub.WCF
{
    public class WcfCallsInterceptor : IInterceptor
    {
        readonly Dictionary<IInvocationMatcher, IInvocationReturnValueProducer> _invocationVersusReturnValue = new Dictionary<IInvocationMatcher, IInvocationReturnValueProducer>();

        public object Fallback { get; set; }

        public void Intercept(IInvocation invocation)
        {
            foreach (var matchVsReturnValue in _invocationVersusReturnValue)
            {
                if (matchVsReturnValue.Key.Matches(invocation.Method, invocation.Arguments))
                {
                    invocation.ReturnValue = matchVsReturnValue.Value.Produce(invocation.Arguments);
                    return;
                }
            }

            if (Fallback != null)
            {
                invocation.ReturnValue = invocation.Method.Invoke(Fallback, invocation.Arguments);
            }
            else
            {
                if (invocation.Method.ReturnType != typeof(void))
                {
                    if (invocation.Method.ReturnType.IsValueType)
                        invocation.ReturnValue = Activator.CreateInstance(invocation.Method.ReturnType);
                    else
                        invocation.ReturnValue = null;
                }
            }
        }

        public void AddInvocation(IInvocationMatcher matcher, IInvocationReturnValueProducer returnValueProducer)
        {
            _invocationVersusReturnValue.Add(matcher, returnValueProducer);        
        }
    }
}