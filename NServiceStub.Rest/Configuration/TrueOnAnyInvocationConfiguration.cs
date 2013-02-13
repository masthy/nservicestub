namespace NServiceStub.Rest.Configuration
{
    public class TrueOnAnyInvocationConfiguration : IGetInvocationConfiguration, IInvocationMatcher
    {
        public static readonly TrueOnAnyInvocationConfiguration Instance = new TrueOnAnyInvocationConfiguration();

        private TrueOnAnyInvocationConfiguration()
        {}

        public IInvocationMatcher CreateInvocationInspector(IRouteTemplate routeToConfigure)
        {
            return this;
        }

        public bool Matches(RequestWrapper request)
        {
            return true;
        }
    }
}