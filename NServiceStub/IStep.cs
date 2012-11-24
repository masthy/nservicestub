namespace NServiceStub
{
    public interface IStep
    {
        void Execute(SequenceExecutionContext context);
    }
}