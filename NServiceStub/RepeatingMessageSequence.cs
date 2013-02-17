using System.Collections.Generic;

namespace NServiceStub
{
    public class RepeatingMessageSequence : IStepConfigurableMessageSequence
    {
        private VerifyExpectation _trigger;
        private readonly StepChain _sequence = new StepChain();
        private readonly List<IStep> _currentSequenceExecutions = new List<IStep>();

        public void ExecuteNextStep(SequenceExecutionContext executionContext)
        {
            if (Trigger.TestExpectationIfAnyMessageOnQueue(executionContext))
            {
                _currentSequenceExecutions.Add(_sequence.Root);
            }

            var currentSteps = new List<IStep>(_currentSequenceExecutions);            
            
            foreach (var currentStep in currentSteps)
            {
                currentStep.Execute(executionContext);
            }

            int index = 0;

            foreach (var currentStep in currentSteps)
            {
                IStep next = _sequence.GetStepAfter(currentStep);

                if (next != null)
                    _currentSequenceExecutions[index] = next;
                else
                    _currentSequenceExecutions.RemoveAt(index--);

                index++;
            }

        }

        public VerifyExpectation Trigger
        {
            get { return _trigger; }
            set { _trigger = value; }
        }

        public void ReplaceStep(IStep stepToReplace, IStep replacement)
        {
            _sequence.ReplaceStep(stepToReplace, replacement);
        }

        public void SetNextStep(IStep nextStep)
        {
            _sequence.SetNextStep(nextStep);
        }
    }
}