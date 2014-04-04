using System;
using System.Linq;
using System.Reflection;

namespace NServiceStub.WCF
{
    public class VoidDelegate : IInvocationVoidCaller
    {
        private readonly Delegate _returnValue;
        private readonly MapInputArgumentHeuristic _mapper;

        public VoidDelegate(Delegate returnValue, MethodInfo inputMethod)
        {
            _returnValue = returnValue;

            _mapper = new MapInputArgumentHeuristic(inputMethod, returnValue);
        }

        public void Call(object[] arguments)
        {
            _returnValue.DynamicInvoke(_mapper.Map(arguments).ToArray());
        }
    }

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