namespace NServiceStub.Rest.Configuration
{
    public class RouteConfiguration<R>
    {
        private readonly IRouteDefinition<R> _routeToConfigure;
        private readonly ServiceStub _service;

        public RouteConfiguration(IRouteDefinition<R> routeToConfigure, ServiceStub service)
        {
            _routeToConfigure = routeToConfigure;
            _service = service;
        }

        public RouteGetConfiguration<R> Setup(IRouteInvocationConfiguration configuration)
        {
            IInvocationMatcher matcher = configuration.CreateInvocationInspector();

            return new RouteGetConfiguration<R>(matcher, _routeToConfigure, _service);
        }
    }
}