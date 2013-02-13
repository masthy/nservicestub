using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace NServiceStub.Rest
{
    public class QueryStringParser
    {
        private const string QueryParameterGroupName = "param";
        private const string QueryParameterValueGroupName = "value";

        public Route Parse(string queryString)
        {
            IEnumerator<char> tokenizer = queryString.GetEnumerator();
            tokenizer.MoveNext();

            return ParseRoute(tokenizer);
        }

        private Route ParseRoute(IEnumerator<char> tokenizer)
        {
            IDictionary<string, string> routeParametersVsNamedGroup = new Dictionary<string, string>();
            IList<string> queryParameters = new List<string>();

            StringBuilder routePattern = ParseRoute(tokenizer, new StringBuilder("^"), routeParametersVsNamedGroup, queryParameters);

            routePattern.Append("$");

            return new Route(new Regex(routePattern.ToString()), routeParametersVsNamedGroup, queryParameters, QueryParameterGroupName, QueryParameterValueGroupName);
        }

        private static StringBuilder ParseRoute(IEnumerator<char> tokenizer, StringBuilder routePattern, IDictionary<string, string> routeParametersVsNamedGroup, IList<string> queryParameters)
        {
            char nextCharacterInRoute = tokenizer.Current;

            if (nextCharacterInRoute == '{')
            {
                tokenizer.MoveNext();
                return ParseRouteParameter(tokenizer, routePattern, routeParametersVsNamedGroup, queryParameters);
            }
            else if (nextCharacterInRoute == '?')
            {
                routePattern.Append(Regex.Escape(nextCharacterInRoute.ToString(CultureInfo.InvariantCulture)));
                tokenizer.MoveNext();
                return ParseQueryStringParameters(tokenizer, routePattern, queryParameters);
            }
            else
            {
                routePattern.Append(Regex.Escape(nextCharacterInRoute.ToString(CultureInfo.InvariantCulture)));
                if (tokenizer.MoveNext())
                    return ParseRoute(tokenizer, routePattern, routeParametersVsNamedGroup, queryParameters);
                else
                    return routePattern;

            }
        }

        private static StringBuilder ParseQueryStringParameters(IEnumerator<char> tokenizer, StringBuilder routePattern, IList<string> queryParameters)
        {
            var parameterName = new StringBuilder();

            bool endOfStream = false;

            while (!endOfStream && tokenizer.Current != '&')
            {
                parameterName.Append(tokenizer.Current);
                endOfStream = !tokenizer.MoveNext();
            }

            string param = parameterName.ToString();
            routePattern.Append(string.Format(@"(?<{0}{2}>[^=\?&]+)=(?<{1}{2}>[^&]+)", QueryParameterGroupName, QueryParameterValueGroupName, queryParameters.Count));
            queryParameters.Add(param);

            if (endOfStream)
                return routePattern;
            else
            {
                routePattern.Append(Regex.Escape(tokenizer.Current.ToString(CultureInfo.InvariantCulture)));
                tokenizer.MoveNext();
                return ParseQueryStringParameters(tokenizer, routePattern, queryParameters);
            }
        }

        private static StringBuilder ParseRouteParameter(IEnumerator<char> tokenizer, StringBuilder routePattern, IDictionary<string, string> routeParameters, IList<string> queryParameters)
        {
            UrlParseHelpers.ParseSingleRouteParameter(tokenizer, routePattern, routeParameters);

            if (tokenizer.MoveNext())
                return ParseRoute(tokenizer, routePattern, routeParameters, queryParameters);
            else
            {
                return routePattern;
            }
        }
    }
}