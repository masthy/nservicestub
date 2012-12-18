namespace NServiceStub.Rest
{
    public class ParameterEqualsValue<T> : IInvocationMatcher
    {
        private readonly T _expectedValue;
        private readonly string _parameterName;
        private readonly ParameterLocation _parameterLocation;

        public ParameterEqualsValue(T expectedValue, string parameterName, ParameterLocation parameterLocation)
        {
            _expectedValue = expectedValue;
            _parameterName = parameterName;
            _parameterLocation = parameterLocation;
        }

        public bool Matches(string rawUrl, IRouteDefinition routeOwningUrl)
        {
            var parameterValue = routeOwningUrl.Route.GetParameterValue<T>(rawUrl, _parameterName, _parameterLocation);

            return _expectedValue.Equals(parameterValue);
        }
    }
}