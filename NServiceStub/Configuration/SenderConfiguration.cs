using System;

namespace NServiceStub.Configuration
{
    public class SenderConfiguration
    {
        private readonly ServiceStub _componentBeingConfigured;
        private readonly IStepConfigurableMessageSequence _sequenceBeingConfigured;
        private readonly IStep _lastSendStep;

        public SenderConfiguration(ServiceStub componentBeingConfigured, IStepConfigurableMessageSequence sequenceBeingConfigured, IStep lastSendStep)
        {
            _componentBeingConfigured = componentBeingConfigured;
            _sequenceBeingConfigured = sequenceBeingConfigured;
            _lastSendStep = lastSendStep;
        }

        public ExpectationConfiguration Expect<T>(Func<T, bool> comparator) where T : class
        {
            return ConfigurationStepCreator.CreateExpectation(_componentBeingConfigured, _sequenceBeingConfigured, comparator);
        }

        public SendMessageExpectedNumberOfTimesConfiguration NumberOfTimes(int numberOfTimesToSendMessage)
        {
            var step = new ExecuteStepNTimes(_lastSendStep, numberOfTimesToSendMessage);
            _sequenceBeingConfigured.ReplaceStep(_lastSendStep, step);
            return new SendMessageExpectedNumberOfTimesConfiguration(_componentBeingConfigured, _sequenceBeingConfigured);
        }

        public SenderConfiguration Send<T>(Action<T> msgInitializer, string destinationQueue) where T : class
        {
            return ConfigurationStepCreator.CreateSendWithNoBind(_componentBeingConfigured, _sequenceBeingConfigured, msgInitializer, destinationQueue);
        }
    }
}