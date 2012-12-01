using System;
using NServiceStub.Configuration;

namespace NServiceStub.WCF.Configuration
{
    public class SendAfterWcfEventConfiguration
    {
        private readonly WcfTriggeredMessageSequence _sequenceBeingConfigured;

        public SendAfterWcfEventConfiguration(WcfTriggeredMessageSequence sequenceBeingConfigured)
        {
            _sequenceBeingConfigured = sequenceBeingConfigured;
        }

        public SenderConfiguration Send<T>(ServiceStub componentBeingConfigured, Action<T> msgInitializer, string destinationQueue) where T : class
        {
            componentBeingConfigured.AddSequence(_sequenceBeingConfigured);

            return ConfigurationStepCreator.Create(componentBeingConfigured, _sequenceBeingConfigured, msgInitializer, destinationQueue);
        }

    }
}