namespace NServiceStub
{
    public interface IMessageSequence
    {
        void ExecuteNextStep(SequenceExecutionContext executionContext);

        bool Done { get; set; }
    }
}