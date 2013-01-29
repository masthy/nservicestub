using System.Net;

namespace NServiceStub.Rest
{
    public interface IRouteTemplate
    {
        bool TryInvocation(HttpListenerRequest request, out object returnValue);

        bool Matches(HttpListenerRequest request);
    }
}