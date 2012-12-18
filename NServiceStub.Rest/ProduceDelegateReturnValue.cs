using System;

namespace NServiceStub.Rest
{
    public class ProduceDelegateReturnValue<R> : IInvocationReturnValueProducer<R>
    {
        private readonly Delegate _returnValueProducer;
        private readonly MapQueryStringDelegateHeuristic _mapper;

        public ProduceDelegateReturnValue(Delegate returnValueProducer, MapQueryStringDelegateHeuristic mapper)
        {
            _returnValueProducer = returnValueProducer;
            _mapper = mapper;
        }

        public R Produce(string rawUrl, IRouteDefinition route)
        {
            return (R)_returnValueProducer.DynamicInvoke(_mapper.Map(rawUrl));
        }
    }
}