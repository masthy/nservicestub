namespace NServiceStub.Rest.Configuration
{
    public class GetTemplateConfiguration<R>
    {
        private readonly IGetTemplate<R> _routeToConfigure;
        private readonly ServiceStub _service;

        public GetTemplateConfiguration(IGetTemplate<R> routeToConfigure, ServiceStub service)
        {
            _routeToConfigure = routeToConfigure;
            _service = service;
        }

        public AfterGetInvocationConfiguration<R> With(IGetInvocationConfiguration configuration)
        {
            IInvocationMatcher matcher = configuration.CreateInvocationInspector(_routeToConfigure);

            return new AfterGetInvocationConfiguration<R>(matcher, _routeToConfigure, _service);
        }
    }
}