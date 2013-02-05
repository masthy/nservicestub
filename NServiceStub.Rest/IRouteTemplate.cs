namespace NServiceStub.Rest
{
    public interface IRouteTemplate
    {
        bool TryInvocation(RequestWrapper request, out object returnValue);

        bool Matches(RequestWrapper request);
    }
}