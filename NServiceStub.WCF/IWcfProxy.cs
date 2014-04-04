namespace NServiceStub.WCF
{
    public interface IWcfProxy
    {
        void AddInvocation(IInvocationMatcher matcher, IInvocationReturnValueProducer returnValueProducer);
        void AddInvocation(IInvocationMatcher matcher, IInvocationVoidCaller voidProduct);
    }
}