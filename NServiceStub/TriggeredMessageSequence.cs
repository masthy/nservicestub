using System.Collections.Generic;

namespace NServiceStub
{
    public class TriggeredMessageSequence : IStepConfigurableMessageSequence
    {
        private readonly StepChain _sequenceOfEvents = new StepChain();
        private readonly List<KeyValuePair<IStep, IMessageInitializerParameterBinder>> _currentSequenceExecutions = new List<KeyValuePair<IStep, IMessageInitializerParameterBinder>>();
        private readonly object _currentSequenceExecutionsLock = new object();

        public void ExecuteNextStep(SequenceExecutionContext executionContext)
        {
            List<KeyValuePair<IStep, IMessageInitializerParameterBinder>> currentStepsSnapshot;
            lock (_currentSequenceExecutionsLock)
            {
                currentStepsSnapshot = new List<KeyValuePair<IStep, IMessageInitializerParameterBinder>>(_currentSequenceExecutions);
            }

            foreach (var currentStep in currentStepsSnapshot)
            {
                executionContext.CapturedInput = currentStep.Value;
                currentStep.Key.Execute(executionContext);
            }

            lock(_currentSequenceExecutionsLock)
            {
                int index = 0;

                foreach (var currentStep in currentStepsSnapshot)
                {
                    IStep next = _sequenceOfEvents.GetStepAfter(currentStep.Key);

                    if (next != null)
                        _currentSequenceExecutions[index] = new KeyValuePair<IStep, IMessageInitializerParameterBinder>(next, _currentSequenceExecutions[index].Value);
                    else
                        _currentSequenceExecutions.Remove(currentStep);
                    index++;
                }
                
            }
        }

        public void TriggerNewSequenceOfEvents(IMessageInitializerParameterBinder capturedArgumentsOfTrigger)
        {
            if (_sequenceOfEvents.Root == null)
                return;

            lock(_currentSequenceExecutionsLock)
            {
                _currentSequenceExecutions.Add(new KeyValuePair<IStep, IMessageInitializerParameterBinder>(_sequenceOfEvents.Root, capturedArgumentsOfTrigger));
            }
        }

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