namespace NServiceStub.Rest.Configuration
{
    public class ParameterEqualsValueConfiguration<T> : IGetOrPostInvocationConfiguration
    {
        private readonly T _expectedValue;
        private readonly string _parameterName;
        private readonly ParameterLocation _parameterLocation;

        public ParameterEqualsValueConfiguration(T expectedValue, string parameterName, ParameterLocation parameterLocation)
        {
            _expectedValue = expectedValue;
            _parameterName = parameterName;
            _parameterLocation = parameterLocation;
        }

        IInvocationMatcher IGetInvocationConfiguration.CreateInvocationInspector(IRouteTemplate routeToConfigure)
        {
            return new ParameterInGetEqualsValue<T>(routeToConfigure, _expectedValue, _parameterName, _parameterLocation);
        }

        IInvocationMatcher IPostInvocationConfiguration.CreateInvocationInspector(IRouteTemplate routeToConfigure)
        {
            return new ParameterInPostEqualsValue<T>(routeToConfigure, _expectedValue, _parameterName, _parameterLocation);
        }
    }
}