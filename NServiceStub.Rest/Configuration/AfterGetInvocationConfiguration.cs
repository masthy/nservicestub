using System;

namespace NServiceStub.Rest.Configuration
{
    public class AfterGetInvocationConfiguration<R>
    {
        private readonly IInvocationMatcher _matcher;
        private readonly IGetTemplate<R> _route;
        private readonly ServiceStub _service;

        public AfterGetInvocationConfiguration(IInvocationMatcher matcher, IGetTemplate<R> route, ServiceStub service)
        {
            _matcher = matcher;
            _route = route;
            _service = service;
        }

        public SendAfterEndpointEventConfiguration Returns(R returnValue)
        {
            var sequence = new TriggeredMessageSequence();
            var inspector = new GetInvocationTriggeringSequenceOfEvents(_route, _matcher, sequence);

            _route.AddReturn(inspector, new ProduceStaticReturnValue<R>(returnValue));

            return new SendAfterEndpointEventConfiguration(sequence, _service);
        }

        public SendAfterEndpointEventConfiguration Returns<T1, T2>(Func<T1, T2, R> returnValueProducer)
        {
            var sequence = new TriggeredMessageSequence();
            var inspector = new GetInvocationTriggeringSequenceOfEvents(_route, _matcher, sequence);

            _route.AddReturn(inspector, new ProduceDelegateReturnValue<R>(returnValueProducer, new MapQueryStringDelegateHeuristic(_route.Route, returnValueProducer)));

            return new SendAfterEndpointEventConfiguration(sequence, _service);
        }
    }
}