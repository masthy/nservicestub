namespace NServiceStub.WCF
{
    public interface IInvocationMatcher
    {
        bool Matches(object[] arguments);
    }
}