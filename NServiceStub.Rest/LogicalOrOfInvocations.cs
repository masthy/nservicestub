using System.Net;

namespace NServiceStub.Rest
{
    public class LogicalOrOfInvocations : IInvocationMatcher
    {
        private readonly IInvocationMatcher _left;
        private readonly IInvocationMatcher _right;

        public LogicalOrOfInvocations(IInvocationMatcher left, IInvocationMatcher right)
        {
            _left = left;
            _right = right;
        }

        public bool Matches(HttpListenerRequest request)
        {
            return _left.Matches(request) || _right.Matches(request);
        }
    }
}