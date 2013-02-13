namespace NServiceStub.Rest.Configuration
{
    public interface IGetInvocationConfiguration
    {
        IInvocationMatcher CreateInvocationInspector(IRouteTemplate routeToConfigure);
    }
}