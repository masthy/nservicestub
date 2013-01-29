using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace NServiceStub.Rest
{
    public class Post : IRoute
    {
        private readonly Regex _rawUrlMatcher;

        public Post(Regex rawUrlMatcher, IDictionary<string, string> routeParametersVsNamedGroup)
        {
            _rawUrlMatcher = rawUrlMatcher;
            RouteParametersInternal = routeParametersVsNamedGroup;
        }

        public IDictionary<string, string> RouteParametersInternal { get; private set; }

        public object GetHeaderParameterValue(string parameterName, NameValueCollection headers, Type expectedType)
        {
            return RouteHelpers.GetHeaderParameterValue(parameterName, headers, expectedType);
        }

        public bool Matches(string rawUrl)
        {
            Match match = _rawUrlMatcher.Match(rawUrl);

            return match.Success;
        }

        public object GetRouteParameterValue(string name, string rawUrl, Type expectedType)
        {
            return RouteHelpers.GetRouteParameterValue(_rawUrlMatcher, RouteParametersInternal, name, rawUrl, expectedType);
        }
    }
}