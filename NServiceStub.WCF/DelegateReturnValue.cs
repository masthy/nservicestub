using System;
using System.Linq;
using System.Reflection;

namespace NServiceStub.WCF
{
    public class DelegateReturnValue : IInvocationReturnValueProducer
    {
        private readonly Delegate _returnValue;
        private readonly MapInputArgumentHeuristic _mapper;

        public DelegateReturnValue(Delegate returnValue, MethodInfo inputMethod)
        {
            _returnValue = returnValue;

            _mapper = new MapInputArgumentHeuristic(inputMethod, returnValue.Method.GetParameters().Select(param => param.ParameterType));
        }

        public object Produce(object[] arguments)
        {
            return _returnValue.DynamicInvoke(_mapper.Map(arguments).ToArray());
        }
    }
}