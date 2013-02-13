namespace NServiceStub.Rest.Configuration
{
    public class InvokeGetConfiguration<R>
    {
        private readonly IRouteTemplate<R> _routeToConfigure;
        private readonly ServiceStub _service;

        public InvokeGetConfiguration(IRouteTemplate<R> routeToConfigure, ServiceStub service)
        {
            _routeToConfigure = routeToConfigure;
            _service = service;
        }

        public ReturnFromGetInvocationConfiguration<R> With(IGetInvocationConfiguration configuration)
        {
            IInvocationMatcher matcher = configuration.CreateInvocationInspector(_routeToConfigure);

            return new ReturnFromGetInvocationConfiguration<R>(matcher, _routeToConfigure, _service);
        }
    }
}