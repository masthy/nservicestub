using System.Reflection;

namespace NServiceStub.WCF
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

        public bool Matches(MethodInfo method, object[] arguments)
        {
            if (_matcher.Matches(method, arguments))
            {
                _sequence.TriggerNewSequenceOfEvents(new CapturedServiceMethodInvocation(InspectedMethod, arguments));
                return true;
            }
            return false;
        }

        public MethodInfo InspectedMethod
        {
            get { return _matcher.InspectedMethod; }
        }
    }
}