using System;
using System.Net;

namespace NServiceStub.Rest
{
    public class ProduceDelegateReturnValue<R> : IInvocationReturnValueProducer<R>
    {
        private readonly Delegate _returnValueProducer;
        private readonly MapRequestToDelegateHeuristic _mapper;

        public ProduceDelegateReturnValue(Delegate returnValueProducer, MapRequestToDelegateHeuristic mapper)
        {
            _returnValueProducer = returnValueProducer;
            _mapper = mapper;
        }

        public R Produce(HttpListenerRequest request)
        {
            return (R)_returnValueProducer.DynamicInvoke(_mapper.Map(request));
        }
    }
}