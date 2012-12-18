using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace NServiceStub.Rest
{
    public class QueryStringParser
    {
        public Route Parse(string queryString)
        {
            IEnumerator<char> tokenizer = queryString.GetEnumerator();
            tokenizer.MoveNext();

            return ParseRoute(tokenizer);
        }

        private Route ParseRoute(IEnumerator<char> tokenizer)
        {
            IDictionary<string, string> routeParametersVsNamedGroup = new Dictionary<string, string>();
            IDictionary<string, string> queryParametersVsNamedGroup = new Dictionary<string, string>();
            StringBuilder routePattern = ParseRoute(tokenizer, new StringBuilder("^"), routeParametersVsNamedGroup, queryParametersVsNamedGroup);

            routePattern.Append("$");

            return new Route(new Regex(routePattern.ToString()), routeParametersVsNamedGroup, queryParametersVsNamedGroup);
        }

        private static StringBuilder ParseRoute(IEnumerator<char> tokenizer, StringBuilder routePattern, IDictionary<string, string> routeParametersVsNamedGroup, IDictionary<string, string> queryParametersVsNamedGroup)
        {
            char nextCharacterInRoute = tokenizer.Current;

            if (nextCharacterInRoute == '{')
            {
                tokenizer.MoveNext();
                return ParseRouteParameter(tokenizer, routePattern, routeParametersVsNamedGroup, queryParametersVsNamedGroup);
            }
            else if (nextCharacterInRoute == '?')
            {
                routePattern.Append(Regex.Escape(nextCharacterInRoute.ToString(CultureInfo.InvariantCulture)));
                tokenizer.MoveNext();
                return ParseQueryStringParameters(tokenizer, routePattern, queryParametersVsNamedGroup);
            }
            else
            {
                routePattern.Append(Regex.Escape(nextCharacterInRoute.ToString(CultureInfo.InvariantCulture)));
                if (tokenizer.MoveNext())
                    return ParseRoute(tokenizer, routePattern, routeParametersVsNamedGroup, queryParametersVsNamedGroup);
                else
                    return routePattern;

            }
        }

        private static StringBuilder ParseQueryStringParameters(IEnumerator<char> tokenizer, StringBuilder routePattern, IDictionary<string, string> queryParameters)
        {
            var parameterName = new StringBuilder();

            bool endOfStream = false;

            while (!endOfStream && tokenizer.Current != '&')
            {
                parameterName.Append(tokenizer.Current);
                routePattern.Append(tokenizer.Current);
                endOfStream = !tokenizer.MoveNext();
            }

            string param = parameterName.ToString();
            queryParameters.Add(param, param);
            routePattern.Append(string.Format("=(?<{0}>[^&]+)", param));

            if (endOfStream)
                return routePattern;
            else
            {
                routePattern.Append(Regex.Escape(tokenizer.Current.ToString(CultureInfo.InvariantCulture)));
                tokenizer.MoveNext();
                return ParseQueryStringParameters(tokenizer, routePattern, queryParameters);
            }
        }

        private static StringBuilder ParseRouteParameter(IEnumerator<char> tokenizer, StringBuilder routePattern, IDictionary<string, string> routeParameters, IDictionary<string, string> queryParameters)
        {
            var parameter = new StringBuilder();

            while (tokenizer.Current != '}')
            {
                parameter.Append(tokenizer.Current);
                tokenizer.MoveNext();
            }

            string param = parameter.ToString();
            routeParameters.Add(param, param);
            routePattern.Append(string.Format("(?<{0}>[^/]+)", param));

            if (tokenizer.Current != '}')
                throw new QueryStringParseException("Expecting } to end route parameter");

            if (tokenizer.MoveNext())
                return ParseRoute(tokenizer, routePattern, routeParameters, queryParameters);
            else
            {
                return routePattern;
            }
        }
    }
}