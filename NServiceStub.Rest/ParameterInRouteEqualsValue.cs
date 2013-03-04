namespace NServiceStub.Rest
{
    public class ParameterInRouteEqualsValue<T> : IInvocationMatcher
    {
        private readonly IRouteTemplate _routeOwningUrl;
        private readonly T _expectedValue;
        private readonly string _parameterName;
        private readonly ParameterLocation _parameterLocation;

        public ParameterInRouteEqualsValue(IRouteTemplate routeOwningUrl, T expectedValue, string parameterName, ParameterLocation parameterLocation)
        {
            _routeOwningUrl = routeOwningUrl;
            _expectedValue = expectedValue;
            _parameterName = parameterName;
            _parameterLocation = parameterLocation;
        }

        public bool Matches(RequestWrapper request)
        {
            var parameterValue = _routeOwningUrl.Route.GetParameterValue<T>(request.Request, _parameterName, _parameterLocation);

            return _expectedValue.Equals(parameterValue);
        }
    }
}