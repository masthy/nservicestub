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

        public IInvocationMatcher CreateInvocationInspector(IRouteTemplate routeToConfigure)
        {
            return new ParameterInRouteEqualsValue<T>(routeToConfigure, _expectedValue, _parameterName, _parameterLocation);
        }

    }
}