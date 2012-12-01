namespace NServiceStub
{
    public interface IStepConfigurableMessageSequence : IMessageSequence
    {
        void ReplaceStep(IStep stepToReplace, IStep replacement);
        void SetNextStep(IStep nextStep);
    }
}