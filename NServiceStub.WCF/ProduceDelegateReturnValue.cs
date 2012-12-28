using System;
using System.Linq;
using System.Reflection;

namespace NServiceStub.WCF
{
    public class ProduceDelegateReturnValue : IInvocationReturnValueProducer
    {
        private readonly Delegate _returnValue;
        private readonly MapInputArgumentHeuristic _mapper;

        public ProduceDelegateReturnValue(Delegate returnValue, MethodInfo inputMethod)
        {
            _returnValue = returnValue;

            _mapper = new MapInputArgumentHeuristic(inputMethod, returnValue);
        }

        public object Produce(object[] arguments)
        {
            return _returnValue.DynamicInvoke(_mapper.Map(arguments).ToArray());
        }
    }
}