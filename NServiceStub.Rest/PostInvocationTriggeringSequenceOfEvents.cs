namespace NServiceStub.Rest
{
    public class PostInvocationTriggeringSequenceOfEvents : IInvocationMatcher
    {
        private readonly IPostTemplate _routeOwningUrl;
        private readonly IInvocationMatcher _matcher;
        private readonly TriggeredMessageSequence _sequence;

        public PostInvocationTriggeringSequenceOfEvents(IPostTemplate routeOwningUrl, IInvocationMatcher matcher, TriggeredMessageSequence sequence)
        {
            _routeOwningUrl = routeOwningUrl;
            _matcher = matcher;
            _sequence = sequence;
        }

        public bool Matches(RequestWrapper request)
        {
            if (_matcher.Matches(request))
            {
                _sequence.TriggerNewSequenceOfEvents(new CapturedPostInvocation(request, _routeOwningUrl));
                return true;
            }
            else
                return false;
        }
    }
}