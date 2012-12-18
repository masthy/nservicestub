namespace NServiceStub.Rest.Configuration
{
    public class ParameterEqualsValueConfiguration<T> : IRouteInvocationConfiguration
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

        public IInvocationMatcher CreateInvocationInspector()
        {
            return new ParameterEqualsValue<T>(_expectedValue, _parameterName, _parameterLocation);
        }
    }
}