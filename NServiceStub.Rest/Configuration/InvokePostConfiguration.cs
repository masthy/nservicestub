namespace NServiceStub.Rest.Configuration
{
    public class InvokePostConfiguration
    {
        private readonly IRouteTemplate _route;
        private readonly ServiceStub _service;

        public InvokePostConfiguration(IRouteTemplate route, ServiceStub service)
        {
            _route = route;
            _service = service;
        }

        public ReturnFromPostInvocationConfiguration With(IPostInvocationConfiguration configuration)
        {
            var sequence = new TriggeredMessageSequence();
            var inspector = new RouteInvocationTriggeringSequenceOfEvents(_route, configuration.CreateInvocationInspector(_route), sequence);

            var returnValueProxy = new NullOrInvocationReturnValueProducer();
            _route.AddReturn(inspector, returnValueProxy);

            return new ReturnFromPostInvocationConfiguration(sequence, _service, returnValueProxy);
        }

    }
}