namespace NServiceStub.WCF
{
    public class ProduceNullReturnValue : IInvocationReturnValueProducer
    {
        public object Produce(object[] arguments)
        {
            return null;
        }
    }
}