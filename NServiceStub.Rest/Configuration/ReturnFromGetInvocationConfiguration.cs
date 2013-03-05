using System;

namespace NServiceStub.Rest.Configuration
{
    public class ReturnFromGetInvocationConfiguration<R>
    {
        private readonly IInvocationMatcher _matcher;
        private readonly IRouteTemplate<R> _route;
        private readonly ServiceStub _service;

        public ReturnFromGetInvocationConfiguration(IInvocationMatcher matcher, IRouteTemplate<R> route, ServiceStub service)
        {
            _matcher = matcher;
            _route = route;
            _service = service;
        }

        public SendAfterEndpointEventConfiguration Returns(R returnValue)
        {
            var sequence = new TriggeredMessageSequence();
            var inspector = new RouteInvocationTriggeringSequenceOfEvents(_route, _matcher, sequence);

            _route.AddReturn(inspector, new ProduceStaticReturnValue(returnValue));

            return new SendAfterEndpointEventConfiguration(sequence, _service);
        }

        public SendAfterEndpointEventConfiguration Returns<T1>(Func<T1, R> returnValueProducer)
        {
            var sequence = new TriggeredMessageSequence();
            var inspector = new RouteInvocationTriggeringSequenceOfEvents(_route, _matcher, sequence);

            _route.AddReturn(inspector, new ProduceDelegateReturnValue(returnValueProducer, new MapRequestToDelegateHeuristic(_route.Route, returnValueProducer)));

            return new SendAfterEndpointEventConfiguration(sequence, _service);
        }

        public SendAfterEndpointEventConfiguration Returns<T1, T2>(Func<T1, T2, R> returnValueProducer)
        {
            var sequence = new TriggeredMessageSequence();
            var inspector = new RouteInvocationTriggeringSequenceOfEvents(_route, _matcher, sequence);

            _route.AddReturn(inspector, new ProduceDelegateReturnValue(returnValueProducer, new MapRequestToDelegateHeuristic(_route.Route, returnValueProducer)));

            return new SendAfterEndpointEventConfiguration(sequence, _service);
        }

        public SendAfterEndpointEventConfiguration Returns<T1, T2, T3>(Func<T1, T2, T3, R> returnValueProducer)
        {
            var sequence = new TriggeredMessageSequence();
            var inspector = new RouteInvocationTriggeringSequenceOfEvents(_route, _matcher, sequence);

            _route.AddReturn(inspector, new ProduceDelegateReturnValue(returnValueProducer, new MapRequestToDelegateHeuristic(_route.Route, returnValueProducer)));

            return new SendAfterEndpointEventConfiguration(sequence, _service);
        }

        public SendAfterEndpointEventConfiguration Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, R> returnValueProducer)
        {
            var sequence = new TriggeredMessageSequence();
            var inspector = new RouteInvocationTriggeringSequenceOfEvents(_route, _matcher, sequence);

            _route.AddReturn(inspector, new ProduceDelegateReturnValue(returnValueProducer, new MapRequestToDelegateHeuristic(_route.Route, returnValueProducer)));

            return new SendAfterEndpointEventConfiguration(sequence, _service);
        }

        public SendAfterEndpointEventConfiguration Returns<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, R> returnValueProducer)
        {
            var sequence = new TriggeredMessageSequence();
            var inspector = new RouteInvocationTriggeringSequenceOfEvents(_route, _matcher, sequence);

            _route.AddReturn(inspector, new ProduceDelegateReturnValue(returnValueProducer, new MapRequestToDelegateHeuristic(_route.Route, returnValueProducer)));

            return new SendAfterEndpointEventConfiguration(sequence, _service);
        }

        public SendAfterEndpointEventConfiguration Returns<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, R> returnValueProducer)
        {
            var sequence = new TriggeredMessageSequence();
            var inspector = new RouteInvocationTriggeringSequenceOfEvents(_route, _matcher, sequence);

            _route.AddReturn(inspector, new ProduceDelegateReturnValue(returnValueProducer, new MapRequestToDelegateHeuristic(_route.Route, returnValueProducer)));

            return new SendAfterEndpointEventConfiguration(sequence, _service);
        }

        public SendAfterEndpointEventConfiguration Returns<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, R> returnValueProducer)
        {
            var sequence = new TriggeredMessageSequence();
            var inspector = new RouteInvocationTriggeringSequenceOfEvents(_route, _matcher, sequence);

            _route.AddReturn(inspector, new ProduceDelegateReturnValue(returnValueProducer, new MapRequestToDelegateHeuristic(_route.Route, returnValueProducer)));

            return new SendAfterEndpointEventConfiguration(sequence, _service);
        }

        public SendAfterEndpointEventConfiguration Returns<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, R> returnValueProducer)
        {
            var sequence = new TriggeredMessageSequence();
            var inspector = new RouteInvocationTriggeringSequenceOfEvents(_route, _matcher, sequence);

            _route.AddReturn(inspector, new ProduceDelegateReturnValue(returnValueProducer, new MapRequestToDelegateHeuristic(_route.Route, returnValueProducer)));

            return new SendAfterEndpointEventConfiguration(sequence, _service);
        }

        public SendAfterEndpointEventConfiguration Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> returnValueProducer)
        {
            var sequence = new TriggeredMessageSequence();
            var inspector = new RouteInvocationTriggeringSequenceOfEvents(_route, _matcher, sequence);

            _route.AddReturn(inspector, new ProduceDelegateReturnValue(returnValueProducer, new MapRequestToDelegateHeuristic(_route.Route, returnValueProducer)));

            return new SendAfterEndpointEventConfiguration(sequence, _service);
        }

        public SendAfterEndpointEventConfiguration Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> returnValueProducer)
        {
            var sequence = new TriggeredMessageSequence();
            var inspector = new RouteInvocationTriggeringSequenceOfEvents(_route, _matcher, sequence);

            _route.AddReturn(inspector, new ProduceDelegateReturnValue(returnValueProducer, new MapRequestToDelegateHeuristic(_route.Route, returnValueProducer)));

            return new SendAfterEndpointEventConfiguration(sequence, _service);
        }

    }
}