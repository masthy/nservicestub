using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;

namespace NServiceStub.Rest
{
    public class MapQueryStringDelegateHeuristic
    {
        private readonly Get _source;

        private readonly List<KeyValuePair<Type, KeyValuePair<string, ParameterLocation>>> _expectedArgumentTypeVsQueryParameter = new List<KeyValuePair<Type, KeyValuePair<string, ParameterLocation>>>();

        public MapQueryStringDelegateHeuristic(Get source, Delegate destination, int skipNumberOfDestinationArguments = 0)
        {
            _source = source;
            BuildMap(source, destination, skipNumberOfDestinationArguments);
        }

        public IEnumerable<object> Map(HttpListenerRequest request)
        {
            var argumentValues = new List<object>();

            foreach (var argumentVsParameter in _expectedArgumentTypeVsQueryParameter)
            {
                object parameterValue = _source.GetParameterValue(request, argumentVsParameter.Key, argumentVsParameter.Value.Key, argumentVsParameter.Value.Value);
                argumentValues.Add(parameterValue);
            }

            return argumentValues;
        }

        private void BuildMap(Get source, Delegate destination, int skipNumberOfDestinationArguments)
        {
            ParameterInfo[] requiredArguments = destination.Method.GetParameters();

            if (requiredArguments.Length == skipNumberOfDestinationArguments)
                return;

            if (MapByArgumentName(source, requiredArguments[skipNumberOfDestinationArguments]))
            {
                foreach (ParameterInfo argument in requiredArguments.Skip(skipNumberOfDestinationArguments + 1))
                    MapByArgumentName(source, argument);
            }
            else
                MapArgumentsByPosition(source, requiredArguments, skipNumberOfDestinationArguments);
        }

        private void MapArgumentsByPosition(Get source, ParameterInfo[] requiredArguments, int skipNumberOfDestinationArguments)
        {
            if ((requiredArguments.Length - skipNumberOfDestinationArguments) != source.QueryParameters.Count() + source.RouteParameters.Count())
                throw new InvalidOperationException("Either use parameter names mathing the query and route parameters or specify as many parameters as the combined number of route and query variables");

            IEnumerator<string> routeParameters = source.RouteParameters.GetEnumerator();
            IEnumerator<string> queryParameters = source.QueryParameters.GetEnumerator();

            foreach (ParameterInfo argument in requiredArguments.Skip(skipNumberOfDestinationArguments))
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

        private bool MapByArgumentName(Get source, ParameterInfo argument)
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
    }
}