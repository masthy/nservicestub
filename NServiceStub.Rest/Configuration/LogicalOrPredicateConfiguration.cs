namespace NServiceStub.Rest.Configuration
{
    public class LogicalOrPredicateConfiguration : IRouteInvocationConfiguration
    {
        public LogicalOrPredicateConfiguration(IRouteInvocationConfiguration left, IRouteInvocationConfiguration right)
        {
            Left = left;
            Right = right;
        }

        public IRouteInvocationConfiguration Left { get; set; }

        public IRouteInvocationConfiguration Right { get; set; }


        public IInvocationMatcher CreateInvocationInspector()
        {
            return new LogicalOrOfInvocations(Left.CreateInvocationInspector(), Right.CreateInvocationInspector());
        }
    }
}