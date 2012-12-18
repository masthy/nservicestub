namespace NServiceStub.Rest
{
    public class InvocationTriggeringSequenceOfEvents : IInvocationMatcher
    {
        private readonly IInvocationMatcher _matcher;
        private readonly TriggeredMessageSequence _sequence;

        public InvocationTriggeringSequenceOfEvents(IInvocationMatcher matcher, TriggeredMessageSequence sequence)
        {
            _matcher = matcher;
            _sequence = sequence;
        }

        public bool Matches(string rawUrl, IRouteDefinition routeOwningUrl)
        {
            if (_matcher.Matches(rawUrl, routeOwningUrl))
            {
                _sequence.TriggerNewSequenceOfEvents();
                return true;
            }
            else
                return false;
        }
    }
}