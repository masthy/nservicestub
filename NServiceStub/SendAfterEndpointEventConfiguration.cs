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

            return ConfigurationStepCreator.CreateSendWithNoBind(_componentBeingConfigured, _sequenceBeingConfigured, msgInitializer, destinationQueue);
        }

        public SenderConfiguration Send<TMsg, TParam1>(Action<TMsg, TParam1> msgInitializer, string destinationQueue) where TMsg : class
        {
            _componentBeingConfigured.AddSequence(_sequenceBeingConfigured);

            return ConfigurationStepCreator.CreateSendWithBind<TMsg>(_componentBeingConfigured, _sequenceBeingConfigured, msgInitializer, destinationQueue);
        }

        public SenderConfiguration Send<TMsg, TParam1, TParam2>(Action<TMsg, TParam1, TParam2> msgInitializer, string destinationQueue) where TMsg : class
        {
            _componentBeingConfigured.AddSequence(_sequenceBeingConfigured);

            return ConfigurationStepCreator.CreateSendWithBind<TMsg>(_componentBeingConfigured, _sequenceBeingConfigured, msgInitializer, destinationQueue);
        }

        public SenderConfiguration Send<TMsg, TParam1, TParam2, TParam3>(Action<TMsg, TParam1, TParam2, TParam3> msgInitializer, string destinationQueue) where TMsg : class
        {
            _componentBeingConfigured.AddSequence(_sequenceBeingConfigured);

            return ConfigurationStepCreator.CreateSendWithBind<TMsg>(_componentBeingConfigured, _sequenceBeingConfigured, msgInitializer, destinationQueue);
        }

        public SenderConfiguration Send<TMsg, TParam1, TParam2, TParam3, TParam4>(Action<TMsg, TParam1, TParam2, TParam3, TParam4> msgInitializer, string destinationQueue) where TMsg : class
        {
            _componentBeingConfigured.AddSequence(_sequenceBeingConfigured);

            return ConfigurationStepCreator.CreateSendWithBind<TMsg>(_componentBeingConfigured, _sequenceBeingConfigured, msgInitializer, destinationQueue);
        }
    }
}