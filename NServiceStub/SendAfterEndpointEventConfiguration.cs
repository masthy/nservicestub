using System;
using NServiceStub.Configuration;

namespace NServiceStub
{
    public class SendAfterEndpointEventConfiguration
    {
        private readonly IStepConfigurableMessageSequence _sequenceBeingConfigured;
        private readonly ServiceStub _componentBeingConfigured;

        public SendAfterEndpointEventConfiguration(IStepConfigurableMessageSequence sequenceBeingConfigured, ServiceStub componentBeingConfigured)
        {
            _sequenceBeingConfigured = sequenceBeingConfigured;
            _componentBeingConfigured = componentBeingConfigured;
        }

        public SenderConfiguration Send<T>(Action<T> msgInitializer, string destinationQueue) where T : class
        {
            _componentBeingConfigured.AddSequence(_sequenceBeingConfigured);

            return ConfigurationStepCreator.Create(_componentBeingConfigured, _sequenceBeingConfigured, msgInitializer, destinationQueue);
        }

    }
}