namespace NServiceStub.Rest
{
    public interface IInvocationMatcher
    {
        bool Matches(string rawUrl, IRouteDefinition routeOwningUrl);
    }
}