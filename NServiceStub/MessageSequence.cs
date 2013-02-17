namespace NServiceStub
{
    public class MessageSequence : IStepConfigurableMessageSequence, INonRepeatingMessageSequence
    {
        private readonly StepChain _chain = new StepChain();

        public void ExecuteNextStep(SequenceExecutionContext executionContext)
        {
            if (Done)
                return;

            if (UpdateExecutionContextWithNextStep(executionContext))
            {
                IStep currentStep = executionContext.GetCurrentStep(this);
                currentStep.Execute(executionContext);
            }
        }

        public void ReplaceStep(IStep stepToReplace, IStep replacement)
        {
            _chain.ReplaceStep(stepToReplace, replacement);
        }

        public void SetNextStep(IStep nextStep)
        {
            _chain.SetNextStep(nextStep);
        }

        public bool Done { get; set; }

        private bool UpdateExecutionContextWithNextStep(SequenceExecutionContext executionContext)
        {
            if (executionContext.GetCurrentStep(this) == null)
                executionContext.SetCurrentStep(this, _chain.Root);
            else
            {
                IStep current = executionContext.GetCurrentStep(this);
                IStep next = _chain.GetStepAfter(current);

                if (next == null)
                {
                    executionContext.Cleanup(this);
                    Done = true;
                }
                else
                    executionContext.SetCurrentStep(this, next);
            }

            return !Done;
        }

    }
}