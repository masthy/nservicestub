using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace NServiceStub.Rest
{
    public class Route
    {
        private readonly string _queryParameterGroupName;
        private readonly string _queryParameterValueGroupName;
        private readonly Regex _rawUrlMatcher;

        public Route(Regex rawUrlMatcher, IDictionary<string, string> routeParametersVsNamedGroup, IList<string> queryParameters, string queryParameterGroupName, string queryParameterValueGroupName)
        {
            _rawUrlMatcher = rawUrlMatcher;
            _queryParameterGroupName = queryParameterGroupName;
            _queryParameterValueGroupName = queryParameterValueGroupName;
            RouteParametersInternal = routeParametersVsNamedGroup;
            QueryParametersInternal = queryParameters;
        }

        public object GetHeaderParameterValue(string parameterName, NameValueCollection headers, Type expectedType)
        {
            string value = headers[parameterName];

            if (value == null)
                return null;

            return Convert.ChangeType(value, expectedType);
        }

        public object GetQueryParameterValue(string name, string rawUrl, Type expectedType)
        {
            Match match = _rawUrlMatcher.Match(rawUrl);

            int? queryParameterMatching = FindIndexOfQueryParameterMatching(match, name);

            if (queryParameterMatching == null)
                throw new ArgumentException(string.Format("Can not find a parameter matching {0}", name), "name");

            return Convert.ChangeType(match.Groups[_queryParameterValueGroupName + queryParameterMatching.Value].Value, expectedType);
        }

        public object GetRouteParameterValue(string name, string rawUrl, Type expectedType)
        {
            string namedGroup = RouteParametersInternal[name];

            Group @group = _rawUrlMatcher.Match(rawUrl).Groups[namedGroup];

            return Convert.ChangeType(@group.Value, expectedType);
        }

        public bool Matches(string rawUrl)
        {
            Match match = _rawUrlMatcher.Match(rawUrl);

            if (!match.Success)
                return false;

            foreach (string parameterName in QueryParametersInternal)
            {
                if (FindIndexOfQueryParameterMatching(match, parameterName) == null)
                    return false;
            }

            return true;
        }

        public IEnumerable<string> QueryParameters
        {
            get { return QueryParametersInternal; }
        }

        public IEnumerable<string> RouteParameters
        {
            get { return RouteParametersInternal.Keys; }
        }

        private IList<string> QueryParametersInternal { get; set; }
        
        private IDictionary<string, string> RouteParametersInternal { get; set; }

        private int? FindIndexOfQueryParameterMatching(Match match, string queryParameterName)
        {
            for (int currentQueryParameter = 0; currentQueryParameter < QueryParametersInternal.Count; currentQueryParameter++)
            {
                if (match.Groups[_queryParameterGroupName + currentQueryParameter].Value == queryParameterName)
                {
                    return currentQueryParameter;
                }
            }

            return null;
        }
    }
}