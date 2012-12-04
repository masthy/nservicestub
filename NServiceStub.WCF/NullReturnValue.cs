namespace NServiceStub.WCF
{
    public class NullReturnValue : IInvocationReturnValueProducer
    {
        public object Produce(object[] arguments)
        {
            return null;
        }
    }
}