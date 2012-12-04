namespace NServiceStub.WCF
{
    public interface IInvocationReturnValueProducer
    {
        object Produce(object[] arguments);
    }
}