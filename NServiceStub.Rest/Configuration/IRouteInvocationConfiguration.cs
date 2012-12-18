namespace NServiceStub.Rest.Configuration
{
    public interface IRouteInvocationConfiguration
    {
        IInvocationMatcher CreateInvocationInspector();
    }
}