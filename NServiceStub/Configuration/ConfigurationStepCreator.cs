using System;

namespace NServiceStub.Configuration
{
    public class ConfigurationStepCreator
    {
        public static ExpectationConfiguration CreateExpectation<T>(ServiceStub componentBeingConfigured, IStepConfigurableMessageSequence sequenceBeingConfigured, Func<T, bool> comparator)
        {
            var nextStep = new VerifyExpectation(sequenceBeingConfigured, new RecievedSingleMessage(Helpers.PackComparatorAsFuncOfObject(comparator)));
            sequenceBeingConfigured.SetNextStep(nextStep);

            return new ExpectationConfiguration(componentBeingConfigured, sequenceBeingConfigured);
        }

        public static SenderConfiguration CreateSendWithNoBind<T>(ServiceStub componentBeingConfigured, IStepConfigurableMessageSequence sequenceBeingConfigured, Action<T> msgInitializer, string destinationQueue)
        {
            var nextStep = new SendMessage(new Sender<T>(componentBeingConfigured.MessageStuffer, destinationQueue, msgInitializer));
            sequenceBeingConfigured.SetNextStep(nextStep);

            return new SenderConfiguration(componentBeingConfigured, sequenceBeingConfigured, nextStep);
        }

        public static SenderConfiguration CreateSendWithBind<TMsg>(ServiceStub componentBeingConfigured, IStepConfigurableMessageSequence sequenceBeingConfigured, Delegate msgInitializer, string destinationQueue)
        {
            var nextStep = new BindParametersAndSendMessage<TMsg>(componentBeingConfigured.MessageStuffer, destinationQueue, msgInitializer);
            sequenceBeingConfigured.SetNextStep(nextStep);

            return new SenderConfiguration(componentBeingConfigured, sequenceBeingConfigured, nextStep);
        }
    }
}