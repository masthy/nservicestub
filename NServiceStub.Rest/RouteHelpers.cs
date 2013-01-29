using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace NServiceStub.Rest
{
    public static class RouteHelpers
    {
        public static object GetRouteParameterValue(Regex rawUrlMatcher, IDictionary<string, string> routeParameters, string name, string rawUrl, Type expectedType)
        {
            string namedGroup = routeParameters[name];

            Group @group = rawUrlMatcher.Match(rawUrl).Groups[namedGroup];

            return Convert.ChangeType(@group.Value, expectedType);
        }

        public static object GetHeaderParameterValue(string parameterName, NameValueCollection headers, Type expectedType)
        {
            string value = headers[parameterName];

            if (value == null)
                return null;

            return Convert.ChangeType(value, expectedType);
        }
    }
}