namespace NServiceStub.WCF
{
    public interface IWcfProxy
    {
        void AddInvocation(IInvocationMatcher matcher, IInvocationReturnValueProducer returnValueProducer);
    }
}