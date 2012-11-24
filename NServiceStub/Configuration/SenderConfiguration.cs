using System;

namespace NServiceStub.Configuration
{
    public class SenderConfiguration
    {
        private readonly ServiceStub _componentBeingConfigured;
        private readonly MessageSequence _sequenceBeingConfigured;
        private readonly SendMessage _lastStep;

        public SenderConfiguration(ServiceStub componentBeingConfigured, MessageSequence sequenceBeingConfigured, SendMessage lastStep)
        {
            _componentBeingConfigured = componentBeingConfigured;
            _sequenceBeingConfigured = sequenceBeingConfigured;
            _lastStep = lastStep;
        }

        public ExpectationConfiguration Expect<T>(Func<T, bool> comparator) where T : class
        {
            return ConfigurationStepCreator.Create(_componentBeingConfigured, _sequenceBeingConfigured, comparator);
        }

        public SendMessageExpectedNumberOfTimesConfiguration NumberOfTimes(int numberOfTimesToSendMessage)
        {
            var step = new SendMessageNTimes(_lastStep, numberOfTimesToSendMessage);
            _sequenceBeingConfigured.ReplaceStep(_lastStep, step);
            return new SendMessageExpectedNumberOfTimesConfiguration(_componentBeingConfigured, _sequenceBeingConfigured);
        }

        public SenderConfiguration Send<T>(Action<T> msgInitializer, string destinationQueue) where T : class
        {
            return ConfigurationStepCreator.Create(_componentBeingConfigured, _sequenceBeingConfigured, msgInitializer, destinationQueue);
        }
    }
}