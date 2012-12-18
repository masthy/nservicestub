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

        public bool Matches(string rawUrl, IRouteDefinition routeOwningUrl)
        {
            return true;
        }
    }
}