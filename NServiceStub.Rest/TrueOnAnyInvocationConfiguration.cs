using System.Net;
using NServiceStub.Rest.Configuration;

namespace NServiceStub.Rest
{
    public class TrueOnAnyInvocationConfiguration : IRouteInvocationConfiguration, IInvocationMatcher
    {
        public static readonly TrueOnAnyInvocationConfiguration Instance = new TrueOnAnyInvocationConfiguration();

        private TrueOnAnyInvocationConfiguration()
        {}

        public IInvocationMatcher CreateInvocationInspector()
        {
            return this;
        }

        public bool Matches(HttpListenerRequest request, IRouteDefinition routeOwningUrl)
        {
            return true;
        }
    }
}