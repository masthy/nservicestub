using System.Net;

namespace NServiceStub.Rest.Configuration
{
    public class NullOrInvocationReturnValueProducer : IInvocationReturnValueProducer
    {
        public IInvocationReturnValueProducer NonNullReturnValue { get; set; }

        public object Produce(HttpListenerRequest request)
        {
            if (NonNullReturnValue == null)
                return null;
            else
                return NonNullReturnValue.Produce(request);
        }
    }
}