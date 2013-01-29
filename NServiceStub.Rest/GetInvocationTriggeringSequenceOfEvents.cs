using System.Net;

namespace NServiceStub.Rest
{
    public class GetInvocationTriggeringSequenceOfEvents : IInvocationMatcher
    {
        private readonly IGetTemplate _routeOwningUrl;
        private readonly IInvocationMatcher _matcher;
        private readonly TriggeredMessageSequence _sequence;

        public GetInvocationTriggeringSequenceOfEvents(IGetTemplate routeOwningUrl, IInvocationMatcher matcher, TriggeredMessageSequence sequence)
        {
            _routeOwningUrl = routeOwningUrl;
            _matcher = matcher;
            _sequence = sequence;
        }

        public bool Matches(HttpListenerRequest request)
        {
            if (_matcher.Matches(request))
            {
                _sequence.TriggerNewSequenceOfEvents(new CapturedGetInvocation(request, _routeOwningUrl));
                return true;
            }
            else
                return false;
        }
    }
}