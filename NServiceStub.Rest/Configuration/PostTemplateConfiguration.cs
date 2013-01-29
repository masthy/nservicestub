namespace NServiceStub.Rest.Configuration
{
    public class PostTemplateConfiguration
    {
        private readonly IPostTemplate _route;
        private readonly ServiceStub _service;

        public PostTemplateConfiguration(IPostTemplate route, ServiceStub service)
        {
            _route = route;
            _service = service;
        }

        public SendAfterEndpointEventConfiguration With(IPostInvocationConfiguration configuration)
        {
            var sequence = new TriggeredMessageSequence();
            var inspector = new PostInvocationTriggeringSequenceOfEvents(_route, configuration.CreateInvocationInspector(_route), sequence);

            _route.AddReturn(inspector, new ProduceStaticReturnValue<object>(null));

            return new SendAfterEndpointEventConfiguration(sequence, _service);
        }

    }
}