namespace NServiceStub.WCF
{
    public class WcfMessageSequenceTrigger : IInvocationMatcher
    {
        private readonly IInvocationMatcher _matcher;
        private readonly WcfTriggeredMessageSequence _sequence;

        public WcfMessageSequenceTrigger(IInvocationMatcher matcher, WcfTriggeredMessageSequence sequence)
        {
            _matcher = matcher;
            _sequence = sequence;
        }

        public bool Matches(object[] arguments)
        {
            if (_matcher.Matches(arguments))
            {
                _sequence.TriggerNewSequenceOfEvents();
                return true;
            }
            return false;
        }
    }
}