using System;

namespace NServiceStub.Configuration
{
    public class SendMessageExpectedNumberOfTimesConfiguration
    {
        private readonly ServiceStub _componentBeingConfigured;
        private readonly IStepConfigurableMessageSequence _sequenceBeingConfigured;

        public SendMessageExpectedNumberOfTimesConfiguration(ServiceStub componentBeingConfigured, IStepConfigurableMessageSequence sequenceBeingConfigured)
        {
            _componentBeingConfigured = componentBeingConfigured;
            _sequenceBeingConfigured = sequenceBeingConfigured;
        }

        public ExpectationConfiguration Expect<T>(Func<T, bool> comparator) where T : class
        {
            return ConfigurationStepCreator.CreateExpectation(_componentBeingConfigured, _sequenceBeingConfigured, comparator);
        }

        public SenderConfiguration Send<T>(Action<T> msgInitializer, string destinationQueue) where T : class
        {
            return ConfigurationStepCreator.CreateSendWithNoBind(_componentBeingConfigured, _sequenceBeingConfigured, msgInitializer, destinationQueue);
        }

    }
}