using System;

namespace NServiceStub.Rest
{
    public static class RouteExtensions
    {
        public static T GetParameterValue<T>(this Route route, string rawUrl, string parameterName, ParameterLocation parameterLocation)
        {
            switch (parameterLocation)
            {
                case ParameterLocation.Query:
                    return route.GetQueryParameterValue<T>(parameterName, rawUrl);
                case ParameterLocation.Route:
                    return route.GetRouteParameterValue<T>(parameterName, rawUrl);
                default:
                    throw new ArgumentOutOfRangeException("parameterLocation");
            }
        }

        public static object GetParameterValue(this Route route, string rawUrl, Type expectedType, string parameterName, ParameterLocation parameterLocation)
        {
            switch (parameterLocation)
            {
                case ParameterLocation.Query:
                    return route.GetQueryParameterValue(parameterName, rawUrl, expectedType);
                case ParameterLocation.Route:
                    return route.GetRouteParameterValue(parameterName, rawUrl, expectedType);
                default:
                    throw new ArgumentOutOfRangeException("parameterLocation");
            }
        }
    }
}