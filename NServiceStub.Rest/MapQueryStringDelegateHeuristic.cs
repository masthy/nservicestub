using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NServiceStub.Rest
{
    public class MapQueryStringDelegateHeuristic
    {
        private readonly Route _source;

        private readonly List<KeyValuePair<Type, KeyValuePair<string, ParameterLocation>>> _expectedArgumentTypeVsQueryParameter = new List<KeyValuePair<Type, KeyValuePair<string, ParameterLocation>>>();

        public MapQueryStringDelegateHeuristic(Route source, Delegate destination)
        {
            _source = source;
            BuildMap(source, destination);
        }

        private void BuildMap(Route source, Delegate destination)
        {
            ParameterInfo[] requiredArguments = destination.Method.GetParameters();

            if (requiredArguments.Length == 0)
                return;
            
            if (MapByArgumentName(source, requiredArguments[0]))
            {
                foreach (var argument in requiredArguments.Skip(1))
                    MapByArgumentName(source, argument);
            }
            else
                MapArgumentsByPosition(source, requiredArguments);
        }

        private void MapArgumentsByPosition(Route source, ParameterInfo[] requiredArguments)
        {
            if (requiredArguments.Length != source.QueryParameters.Count() + source.RouteParameters.Count())
                throw new InvalidOperationException("Either use parameter names mathing the query and route parameters or specify as many parameters as the combined number of route and query variables");

            IEnumerator<string> routeParameters = source.RouteParameters.GetEnumerator();
            IEnumerator<string> queryParameters = source.QueryParameters.GetEnumerator();

            foreach (var argument in requiredArguments)
            {
                if (routeParameters.MoveNext())
                {
                    _expectedArgumentTypeVsQueryParameter.Add(new KeyValuePair<Type, KeyValuePair<string, ParameterLocation>>(argument.ParameterType,
                                                                                                                              new KeyValuePair<string, ParameterLocation>(routeParameters.Current, ParameterLocation.Route)));
                }
                else
                {
                    queryParameters.MoveNext();
                    _expectedArgumentTypeVsQueryParameter.Add(new KeyValuePair<Type, KeyValuePair<string, ParameterLocation>>(argument.ParameterType,
                                                                                                                              new KeyValuePair<string, ParameterLocation>(queryParameters.Current, ParameterLocation.Query)));
                }
            }
        }

        private bool MapByArgumentName(Route source, ParameterInfo argument)
        {
            string argumentName = argument.Name;

            if (source.RouteParameters.Any(parameter => parameter == argumentName))
            {
                _expectedArgumentTypeVsQueryParameter.Add(new KeyValuePair<Type, KeyValuePair<string, ParameterLocation>>(argument.ParameterType, new KeyValuePair<string, ParameterLocation>(argumentName, ParameterLocation.Route)));
                return true;
            }
            else if (source.QueryParameters.Any(parameter => parameter == argumentName))
            {
                _expectedArgumentTypeVsQueryParameter.Add(new KeyValuePair<Type, KeyValuePair<string, ParameterLocation>>(argument.ParameterType, new KeyValuePair<string, ParameterLocation>(argumentName, ParameterLocation.Query)));
                return true;
            }
            return false;
        }

        public object[] Map(string rawUrl)
        {
            var argumentValues = new List<object>();

            foreach (var argumentVsParameter in _expectedArgumentTypeVsQueryParameter)
            {
                object parameterValue = _source.GetParameterValue(rawUrl, argumentVsParameter.Key, argumentVsParameter.Value.Key, argumentVsParameter.Value.Value);
                argumentValues.Add(parameterValue);
            }

            return argumentValues.ToArray();
        }
    }
}