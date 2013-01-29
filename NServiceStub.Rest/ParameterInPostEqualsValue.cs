using System;
using System.Net;

namespace NServiceStub.Rest
{
    public class ParameterInPostEqualsValue<T> : IInvocationMatcher
    {
        private readonly IPostTemplate _routeOwningUrl;
        private readonly T _expectedValue;
        private readonly string _parameterName;
        private readonly ParameterLocation _parameterLocation;

        public ParameterInPostEqualsValue(IPostTemplate routeOwningUrl, T expectedValue, string parameterName, ParameterLocation parameterLocation)
        {
            if (parameterLocation == ParameterLocation.Query)
                throw new ArgumentOutOfRangeException("parameterLocation", "Query parameters not supported");

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