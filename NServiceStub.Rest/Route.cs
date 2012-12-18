using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NServiceStub.Rest
{
    public class Route
    {
        private readonly Regex _rawUrlMatcher;

        public Route(Regex rawUrlMatcher, IDictionary<string, string> routeParametersVsNamedGroup, IDictionary<string, string> queryParametersVsNamedGroup)
        {
            _rawUrlMatcher = rawUrlMatcher;
            RouteParametersInternal = routeParametersVsNamedGroup;
            QueryParametersInternal = queryParametersVsNamedGroup;
        }

        public bool Matches(string rawUrl)
        {
            return _rawUrlMatcher.IsMatch(rawUrl);
        }

        public IEnumerable<string> RouteParameters
        {
            get { return RouteParametersInternal.Keys; }
        }

        public IEnumerable<string> QueryParameters
        {
            get { return QueryParametersInternal.Keys; }
        }

        private IDictionary<string, string> RouteParametersInternal { get; set; }

        private IDictionary<string, string> QueryParametersInternal { get; set; }

        public T GetQueryParameterValue<T>(string name, string rawUrl)
        {
            return (T)GetQueryParameterValue(name, rawUrl, typeof(T));
        }

        public T GetRouteParameterValue<T>(string name, string rawUrl)
        {
            return (T)GetRouteParameterValue(name, rawUrl, typeof(T));
        }

        public object GetQueryParameterValue(string name, string rawUrl, Type expectedType)
        {
            string namedGroup = QueryParametersInternal[name];

            Group @group = _rawUrlMatcher.Match(rawUrl).Groups[namedGroup];

            return Convert.ChangeType(@group.Value, expectedType);
        }

        public object GetRouteParameterValue(string name, string rawUrl, Type expectedType)
        {
            string namedGroup = RouteParametersInternal[name];

            Group @group = _rawUrlMatcher.Match(rawUrl).Groups[namedGroup];

            return Convert.ChangeType(@group.Value, expectedType);
        }

    }
}