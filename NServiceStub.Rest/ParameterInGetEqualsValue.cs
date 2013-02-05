﻿namespace NServiceStub.Rest
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

        public bool Matches(RequestWrapper request)
        {
            var parameterValue = _routeOwningUrl.Route.GetParameterValue<T>(request.Request, _parameterName, _parameterLocation);

            return _expectedValue.Equals(parameterValue);
        }
    }
}