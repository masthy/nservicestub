using System;
using System.Net;

namespace NServiceStub.Rest
{
    public static class RouteExtensions
    {
        public static T GetParameterValue<T>(this IRoute route, HttpListenerRequest request, string parameterName, ParameterLocation parameterLocation)
        {
            object parameterValue = route.GetParameterValue(request, typeof(T), parameterName, parameterLocation);

            if (parameterValue == null)
                return default(T);
            else
                return (T)parameterValue;
        }

        public static object GetParameterValue(this IRoute route, HttpListenerRequest request, Type expectedType, string parameterName, ParameterLocation parameterLocation)
        {
            switch (parameterLocation)
            {
                case ParameterLocation.Query:
                    return route.GetQueryParameterValue(parameterName, request.RawUrl, expectedType);
                case ParameterLocation.Route:
                    return route.GetRouteParameterValue(parameterName, request.RawUrl, expectedType);
                case ParameterLocation.Header:
                    return route.GetHeaderParameterValue(parameterName, request.Headers, expectedType);
                default:
                    throw new ArgumentOutOfRangeException("parameterLocation");
            }
        }

    }
}