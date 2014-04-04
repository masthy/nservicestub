namespace NServiceStub.WCF
{
    public interface IInvocationVoidCaller
    {
        void Call(object[] arguments);
    }

    public interface IInvocationReturnValueProducer
    {
        object Produce(object[] arguments);
    }
}