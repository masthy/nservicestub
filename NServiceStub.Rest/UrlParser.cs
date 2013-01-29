using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace NServiceStub.Rest
{
    public class UrlParser
    {
        public Post Parse(string queryString)
        {
            IEnumerator<char> tokenizer = queryString.GetEnumerator();
            tokenizer.MoveNext();

            return ParseRoute(tokenizer);
        }

        private Post ParseRoute(IEnumerator<char> tokenizer)
        {
            IDictionary<string, string> routeParametersVsNamedGroup = new Dictionary<string, string>();

            StringBuilder routePattern = ParseRoute(tokenizer, new StringBuilder("^"), routeParametersVsNamedGroup);

            routePattern.Append("$");

            return new Post(new Regex(routePattern.ToString()), routeParametersVsNamedGroup);
        }

        private static StringBuilder ParseRoute(IEnumerator<char> tokenizer, StringBuilder routePattern, IDictionary<string, string> routeParametersVsNamedGroup)
        {
            char nextCharacterInRoute = tokenizer.Current;

            if (nextCharacterInRoute == '{')
            {
                tokenizer.MoveNext();
                return ParseRouteParameter(tokenizer, routePattern, routeParametersVsNamedGroup);
            }
            else
            {
                routePattern.Append(Regex.Escape(nextCharacterInRoute.ToString(CultureInfo.InvariantCulture)));
                if (tokenizer.MoveNext())
                    return ParseRoute(tokenizer, routePattern, routeParametersVsNamedGroup);
                else
                    return routePattern;

            }
        }

        private static StringBuilder ParseRouteParameter(IEnumerator<char> tokenizer, StringBuilder routePattern, IDictionary<string, string> routeParameters)
        {
            UrlParseHelpers.ParseSingleRouteParameter(tokenizer, routePattern, routeParameters);

            if (tokenizer.MoveNext())
                return ParseRoute(tokenizer, routePattern, routeParameters);
            else
                return routePattern;
        }
 
    }
}