namespace NServiceStub
{
    public interface IMessageSequence
    {
        void ExecuteNextStep(SequenceExecutionContext executionContext);
    }
}