using System.Net;
using NServiceStub.Rest.Configuration;

namespace NServiceStub.Rest
{
    public class TrueOnAnyInvocationConfiguration : IGetInvocationConfiguration, IInvocationMatcher
    {
        public static readonly TrueOnAnyInvocationConfiguration Instance = new TrueOnAnyInvocationConfiguration();

        private TrueOnAnyInvocationConfiguration()
        {}

        public IInvocationMatcher CreateInvocationInspector(IGetTemplate routeToConfigure)
        {
            return this;
        }

        public bool Matches(HttpListenerRequest request)
        {
            return true;
        }
    }
}