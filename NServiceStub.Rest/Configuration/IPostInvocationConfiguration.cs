namespace NServiceStub.Rest.Configuration
{
    public interface IPostInvocationConfiguration
    {
        IInvocationMatcher CreateInvocationInspector(IRouteTemplate routeToConfigure);         
    }
}