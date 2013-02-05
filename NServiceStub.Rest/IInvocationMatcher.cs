using System.Net;

namespace NServiceStub.Rest
{
    public interface IInvocationMatcher
    {
        bool Matches(RequestWrapper request);
    }
}