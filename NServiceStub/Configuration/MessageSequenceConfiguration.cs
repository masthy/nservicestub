using System;

namespace NServiceStub.Configuration
{
    public class MessageSequenceConfiguration
    {
        private readonly ServiceStub _componentBeingConfigured;
        private readonly MessageSequence _sequenceBeingConfigured;

        public MessageSequenceConfiguration(ServiceStub componentBeingConfigured, MessageSequence sequenceBeingConfigured)
        {
            _componentBeingConfigured = componentBeingConfigured;
            _sequenceBeingConfigured = sequenceBeingConfigured;
        }

        public ExpectationConfiguration Expect<T>(Func<T, bool> comparator) where T : class
        {
            return ConfigurationStepCreator.Create(_componentBeingConfigured, _sequenceBeingConfigured, comparator);
        }

        public SenderConfiguration Send<T>(Action<T> msgInitializer, string destinationQueue) where T : class
        {
            return ConfigurationStepCreator.Create(_componentBeingConfigured, _sequenceBeingConfigured, msgInitializer, destinationQueue);
        }
    }
}