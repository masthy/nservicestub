using System.Net;

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

        public bool Matches(HttpListenerRequest request, IRouteDefinition routeOwningUrl)
        {
            if (_matcher.Matches(request, routeOwningUrl))
            {
                _sequence.TriggerNewSequenceOfEvents(new CapturedRouteInvocation(request, routeOwningUrl));
                return true;
            }
            else
                return false;
        }
    }
}