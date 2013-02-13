namespace NServiceStub.Rest
{
    public class RouteInvocationTriggeringSequenceOfEvents : IInvocationMatcher
    {
        private readonly IRouteTemplate _routeOwningUrl;
        private readonly IInvocationMatcher _matcher;
        private readonly TriggeredMessageSequence _sequence;

        public RouteInvocationTriggeringSequenceOfEvents(IRouteTemplate routeOwningUrl, IInvocationMatcher matcher, TriggeredMessageSequence sequence)
        {
            _routeOwningUrl = routeOwningUrl;
            _matcher = matcher;
            _sequence = sequence;
        }

        public bool Matches(RequestWrapper request)
        {
            if (_matcher.Matches(request))
            {
                _sequence.TriggerNewSequenceOfEvents(new CapturedRouteInvocation(request, _routeOwningUrl));
                return true;
            }
            else
                return false;
        }
    }
}