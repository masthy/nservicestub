using System;

namespace NServiceStub.Configuration
{
    public class ExpectationConfiguration
    {
        private readonly ServiceStub _componentBeingConfigured;
        private readonly IStepConfigurableMessageSequence _sequenceBeingConfigured;

        public ExpectationConfiguration(ServiceStub componentBeingConfigured, IStepConfigurableMessageSequence sequenceBeingConfigured)
        {
            _componentBeingConfigured = componentBeingConfigured;
            _sequenceBeingConfigured = sequenceBeingConfigured;
        }

        public SenderConfiguration Send<T>(Action<T> msgInitializer, string destinationQueue) where T : class
        {
            var thisStep = new SendMessage(new Sender<T>(_componentBeingConfigured.MessageStuffer, destinationQueue, msgInitializer));
            _sequenceBeingConfigured.SetNextStep(thisStep);

            return new SenderConfiguration(_componentBeingConfigured, _sequenceBeingConfigured, thisStep);
        }
    }
}