namespace NServiceStub.WCF
{
    public class WcfMessageSequenceTrigger : IInvocationMatcher
    {
        private readonly IInvocationMatcher _matcher;
        private readonly TriggeredMessageSequence _sequence;

        public WcfMessageSequenceTrigger(IInvocationMatcher matcher, TriggeredMessageSequence sequence)
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