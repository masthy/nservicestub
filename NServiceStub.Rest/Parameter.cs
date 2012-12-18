using NServiceStub.Rest.Configuration;

namespace NServiceStub.Rest
{
    public class Parameter
    {
        public static ParameterConfiguration<T> RouteParameter<T>(string name)
        {
            return new ParameterConfiguration<T>(ParameterLocation.Route, name);
        }

        public static ParameterConfiguration<T> QueryParameter<T>(string name)
        {
            return new ParameterConfiguration<T>(ParameterLocation.Query, name);            
        }

        public static IRouteInvocationConfiguration Any()
        {
            return TrueOnAnyInvocationConfiguration.Instance;
        }
    }
}