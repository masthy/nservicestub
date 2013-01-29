namespace NServiceStub.Rest.Configuration
{
    public interface IGetInvocationConfiguration
    {
        IInvocationMatcher CreateInvocationInspector(IGetTemplate routeToConfigure);
    }
}