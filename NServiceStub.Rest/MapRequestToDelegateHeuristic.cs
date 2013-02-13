using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Reflection;

namespace NServiceStub.Rest
{
    public class MapRequestToDelegateHeuristic
    {
        private readonly Route _source;
        private readonly Delegate _destination;
        private readonly int _skipNumberOfDestinationArguments;

        private List<KeyValuePair<Type, ParameterNameAndLocation>> _map;

        public MapRequestToDelegateHeuristic(Route source, Delegate destination, int skipNumberOfDestinationArguments = 0)
        {
            _source = source;
            _destination = destination;
            _skipNumberOfDestinationArguments = skipNumberOfDestinationArguments;
        }

        public IEnumerable<object> Map(HttpListenerRequest request)
        {
            if (_map == null)
                _map = BuildMap(_source, _destination, request.Headers, _skipNumberOfDestinationArguments);

            var argumentValues = new List<object>();

            foreach (var argumentVsParameter in _map)
            {
                object parameterValue = _source.GetParameterValue(request, argumentVsParameter.Key, argumentVsParameter.Value.Name, argumentVsParameter.Value.Location);
                argumentValues.Add(parameterValue);
            }

            return argumentValues;
        }

        private List<KeyValuePair<Type, ParameterNameAndLocation>> BuildMap(Route source, Delegate destination, NameValueCollection headers, int skipNumberOfDestinationArguments)
        {
            ParameterInfo[] requiredArguments = destination.Method.GetParameters();
            var map = new List<KeyValuePair<Type, ParameterNameAndLocation>>();

            if (requiredArguments.Length == skipNumberOfDestinationArguments)
                return map;

            bool mappedSuccessfullyByName = MapByArgumentName(source, requiredArguments[skipNumberOfDestinationArguments], headers, map);

            if (mappedSuccessfullyByName)
            {
                foreach (ParameterInfo argument in requiredArguments.Skip(skipNumberOfDestinationArguments + 1))
                    mappedSuccessfullyByName &= MapByArgumentName(source, argument, headers, map);
            }
            
            if (!mappedSuccessfullyByName)
            {
                map.Clear();
                MapArgumentsByPosition(source, requiredArguments, headers, skipNumberOfDestinationArguments, map);
            }

            return map;
        }

        private void MapArgumentsByPosition(Route source, ParameterInfo[] requiredArguments, NameValueCollection headers, int skipNumberOfDestinationArguments, List<KeyValuePair<Type, ParameterNameAndLocation>> map)
        {
            if ((requiredArguments.Length - skipNumberOfDestinationArguments) != source.QueryParameters.Count() + source.RouteParameters.Count())
                throw new InvalidOperationException("Either use parameter names mathing the query and route parameters or specify as many parameters as the combined number of route and query variables");

            IEnumerator headerParameters = headers.AllKeys.GetEnumerator(); 
            IEnumerator<string> routeParameters = source.RouteParameters.GetEnumerator();
            IEnumerator<string> queryParameters = source.QueryParameters.GetEnumerator();

            foreach (ParameterInfo argument in requiredArguments.Skip(skipNumberOfDestinationArguments))
            {
                if (headerParameters.MoveNext())
                {
                    object value = RouteHelpers.GetHeaderParameterValue((string)headerParameters.Current, headers, argument.ParameterType);

                    if (value != null)
                    {
                        map.Add(new KeyValuePair<Type, ParameterNameAndLocation>(argument.ParameterType,
                                                                                                                                  new ParameterNameAndLocation { Name = routeParameters.Current, Location = ParameterLocation.Header }));                        
                    }
                }

                if (routeParameters.MoveNext())
                {
                    map.Add(new KeyValuePair<Type, ParameterNameAndLocation>(argument.ParameterType,
                                                                                                                              new ParameterNameAndLocation{ Name = routeParameters.Current, Location = ParameterLocation.Route }));
                }
                else
                {
                    queryParameters.MoveNext();
                    map.Add(new KeyValuePair<Type, ParameterNameAndLocation>(argument.ParameterType,
                                                                                                                              new ParameterNameAndLocation{ Name = queryParameters.Current, Location = ParameterLocation.Query }));
                }
            }
        }

        private bool MapByArgumentName(Route source, ParameterInfo argument, NameValueCollection headers, IList<KeyValuePair<Type, ParameterNameAndLocation>> map)
        {
            string argumentName = argument.Name;

            if (headers.AllKeys.Any(key => key.Equals(argumentName, StringComparison.InvariantCultureIgnoreCase)))
            {
                map.Add(new KeyValuePair<Type, ParameterNameAndLocation>(argument.ParameterType, new ParameterNameAndLocation { Name = argumentName, Location = ParameterLocation.Header }));
                return true;
            }
            else if (source.RouteParameters.Any(parameter => parameter == argumentName))
            {
                map.Add(new KeyValuePair<Type, ParameterNameAndLocation>(argument.ParameterType, new ParameterNameAndLocation { Name = argumentName, Location = ParameterLocation.Route }));
                return true;
            }
            else if (source.QueryParameters.Any(parameter => parameter == argumentName))
            {
                map.Add(new KeyValuePair<Type, ParameterNameAndLocation>(argument.ParameterType, new ParameterNameAndLocation { Name = argumentName, Location = ParameterLocation.Query }));
                return true;
            }
            return false;
        }
    }
}