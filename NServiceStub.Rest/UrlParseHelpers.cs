using System;
using System.Collections.Generic;
using System.Text;

namespace NServiceStub.Rest
{
    public static class UrlParseHelpers
    {
        public static void ParseSingleRouteParameter(IEnumerator<char> tokenizer, StringBuilder routePattern, IDictionary<string, string> routeParameters)
        {
            var parameter = new StringBuilder();

            while (tokenizer.Current != '}')
            {
                parameter.Append(tokenizer.Current);
                tokenizer.MoveNext();
            }

            string param = parameter.ToString();
            routeParameters.Add(param, param);
            routePattern.Append(String.Format("(?<{0}>[^/]+)", param));

            if (tokenizer.Current != '}')
                throw new QueryStringParseException("Expecting } to end route parameter");
        }
    }
}