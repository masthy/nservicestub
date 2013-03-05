using System;
using System.Linq;
using System.Net;

namespace NServiceStub.Rest
{
    public class ProduceDelegateReturnValue : IInvocationReturnValueProducer
    {
        private readonly Delegate _returnValueProducer;
        private readonly MapRequestToDelegateHeuristic _mapper;

        public ProduceDelegateReturnValue(Delegate returnValueProducer, MapRequestToDelegateHeuristic mapper)
        {
            _returnValueProducer = returnValueProducer;
            _mapper = mapper;
        }

        public object Produce(HttpListenerRequest request)
        {
            return _returnValueProducer.DynamicInvoke(_mapper.Map(request).ToArray());
        }
    }
}