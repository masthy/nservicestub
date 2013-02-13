namespace NServiceStub.Rest.Configuration
{
    public class LogicalOrPredicateConfiguration : IGetOrPostInvocationConfiguration
    {
        public LogicalOrPredicateConfiguration(IGetOrPostInvocationConfiguration left, IGetOrPostInvocationConfiguration right)
        {
            Left = left;
            Right = right;
        }

        public IGetOrPostInvocationConfiguration Left { get; set; }

        public IGetOrPostInvocationConfiguration Right { get; set; }

        IInvocationMatcher IGetInvocationConfiguration.CreateInvocationInspector(IRouteTemplate routeToConfigure)
        {
            return new LogicalOrOfInvocations(Left.AsGetConfiguration().CreateInvocationInspector(routeToConfigure), Right.AsGetConfiguration().CreateInvocationInspector(routeToConfigure));
        }

        IInvocationMatcher IPostInvocationConfiguration.CreateInvocationInspector(IRouteTemplate routeToConfigure)
        {
            return new LogicalOrOfInvocations(Left.AsPostConfiguration().CreateInvocationInspector(routeToConfigure), Right.AsPostConfiguration().CreateInvocationInspector(routeToConfigure));
        }
    }
}