using System;

namespace NServiceStub.Configuration
{
    public class MessageSequenceConfiguration
    {
        private readonly ServiceStub _componentBeingConfigured;

        public MessageSequenceConfiguration(ServiceStub componentBeingConfigured)
        {
            _componentBeingConfigured = componentBeingConfigured;
        }

        public ExpectationConfiguration Expect<T>(Func<T, bool> comparator) where T : class
        {
            var sequence = new RepeatingMessageSequence();
            _componentBeingConfigured.AddSequence(sequence);

            var nextStep = new VerifyExpectation(sequence, new RecievedSingleMessage(Helpers.PackComparatorAsFuncOfObject(comparator)));
            sequence.Trigger = nextStep;

            return new ExpectationConfiguration(_componentBeingConfigured, sequence);
        }

        public SenderConfiguration Send<T>(Action<T> msgInitializer, string destinationQueue) where T : class
        {
            var sequence = new MessageSequence();
            _componentBeingConfigured.AddSequence(sequence);

            return ConfigurationStepCreator.CreateSendWithNoBind(_componentBeingConfigured, sequence, msgInitializer, destinationQueue);
        }
    }
}