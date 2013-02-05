namespace NServiceStub.Rest.Configuration
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

        public static ParameterConfiguration<T> HeaderParameter<T>(string name)
        {
            return new ParameterConfiguration<T>(ParameterLocation.Header, name);
        }

        public static IGetInvocationConfiguration Any()
        {
            return TrueOnAnyInvocationConfiguration.Instance;
        }
    }
}