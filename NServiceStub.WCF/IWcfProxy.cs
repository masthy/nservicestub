using System;

namespace NServiceStub.WCF
{
    public interface IWcfProxy
    {
        void AddInvocation(IInvocationMatcher matcher, Func<object> returnValueProducer);
    }
}