using System.Collections.Generic;

namespace NServiceStub.WCF
{
    public class WcfTriggeredMessageSequence : IStepConfigurableMessageSequence
    {
        private readonly StepChain _sequenceOfEvents = new StepChain();

        readonly List<IStep> _currentSequenceExecutions = new List<IStep>();

        public void ExecuteNextStep(SequenceExecutionContext executionContext)
        {
            var currentSteps = new List<IStep>(_currentSequenceExecutions);
            foreach (var currentStep in currentSteps)
            {
                currentStep.Execute(executionContext);
            }

            int index = 0;

            foreach (var currentStep in currentSteps)
            {
                IStep next = _sequenceOfEvents.GetStepAfter(currentStep);

                if (next != null)
                    _currentSequenceExecutions[index] = next;
                else
                    _currentSequenceExecutions.Remove(currentStep);
                index++;
            }

        }

        public void TriggerNewSequenceOfEvents()
        {
            if (_sequenceOfEvents.Root == null)
                return;

            _currentSequenceExecutions.Add(_sequenceOfEvents.Root);
        }

        public bool Done { get; set; }

        public void ReplaceStep(IStep stepToReplace, IStep replacement)
        {
            _sequenceOfEvents.ReplaceStep(stepToReplace, replacement);
        }

        public void SetNextStep(IStep nextStep)
        {
            _sequenceOfEvents.SetNextStep(nextStep);
        }
    }
}