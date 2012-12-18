using System;

namespace NServiceStub.Rest
{
    public class ParameterEqualsPredicate<T> : IInvocationMatcher
    {
        private readonly Func<T, bool> _predicate;
        private readonly ParameterLocation _parameterLocation;
        private readonly string _parameterName;

        public ParameterEqualsPredicate(Func<T, bool> predicate, ParameterLocation parameterLocation, string parameterName)
        {
            _predicate = predicate;
            _parameterLocation = parameterLocation;
            _parameterName = parameterName;
        }

        public bool Matches(string rawUrl, IRouteDefinition routeOwningUrl)
        {
            var parameterValue = routeOwningUrl.Route.GetParameterValue<T>(rawUrl, _parameterName, _parameterLocation);

            return _predicate(parameterValue);
        }

    }
}