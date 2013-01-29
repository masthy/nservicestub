using System.Net;

namespace NServiceStub.Rest
{
    public class ParameterInGetEqualsValue<T> : IInvocationMatcher
    {
        private readonly IGetTemplate _routeOwningUrl;
        private readonly T _expectedValue;
        private readonly string _parameterName;
        private readonly ParameterLocation _parameterLocation;

        public ParameterInGetEqualsValue(IGetTemplate routeOwningUrl, T expectedValue, string parameterName, ParameterLocation parameterLocation)
        {
            _routeOwningUrl = routeOwningUrl;
            _expectedValue = expectedValue;
            _parameterName = parameterName;
            _parameterLocation = parameterLocation;
        }

        public bool Matches(HttpListenerRequest request)
        {
            var parameterValue = _routeOwningUrl.Route.GetParameterValue<T>(request, _parameterName, _parameterLocation);

            return _expectedValue.Equals(parameterValue);
        }
    }
}