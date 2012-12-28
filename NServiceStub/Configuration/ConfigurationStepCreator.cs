using System;

namespace NServiceStub.Configuration
{
    public class ConfigurationStepCreator
    {
        public static ExpectationConfiguration Create<T>(ServiceStub componentBeingConfigured, IStepConfigurableMessageSequence sequenceBeingConfigured, Func<T, bool> comparator)
        {
            var nextStep = new VerifyExpectation(sequenceBeingConfigured, new RecievedSingleMessage(Helpers.PackComparatorAsFuncOfObject(comparator)));
            sequenceBeingConfigured.SetNextStep(nextStep);

            return new ExpectationConfiguration(componentBeingConfigured, sequenceBeingConfigured);
        }

        public static SenderConfiguration Create<T>(ServiceStub componentBeingConfigured, IStepConfigurableMessageSequence sequenceBeingConfigured, Action<T> msgInitializer, string destinationQueue)
        {
            var nextStep = new SendMessage(new Sender<T>(componentBeingConfigured.MessageStuffer, destinationQueue, msgInitializer));
            sequenceBeingConfigured.SetNextStep(nextStep);

            return new SenderConfiguration(componentBeingConfigured, sequenceBeingConfigured, nextStep);
        }

        public static SenderConfiguration Create<TMsg, TParam>(ServiceStub componentBeingConfigured, IStepConfigurableMessageSequence sequenceBeingConfigured, Action<TMsg, TParam> msgInitializer, string destinationQueue)
        {
            var nextStep = new BindParametersAndSendMessage<TMsg>(componentBeingConfigured.MessageStuffer, destinationQueue, msgInitializer);
            sequenceBeingConfigured.SetNextStep(nextStep);

            return new SenderConfiguration(componentBeingConfigured, sequenceBeingConfigured, nextStep);
        }
    }
}