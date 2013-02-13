using System.Net;

namespace NServiceStub.Rest
{
    public class ProduceStaticReturnValue : IInvocationReturnValueProducer
    {
        private readonly object _returnValue;

        public ProduceStaticReturnValue(object returnValue)
        {
            _returnValue = returnValue;
        }

        public object Produce(HttpListenerRequest request)
        {
            return _returnValue;
        }
    }
}