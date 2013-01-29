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

        IInvocationMatcher IGetInvocationConfiguration.CreateInvocationInspector(IGetTemplate routeToConfigure)
        {
            return new LogicalOrOfInvocations(Left.CreateInvocationInspector(routeToConfigure), Right.CreateInvocationInspector(routeToConfigure));
        }

        IInvocationMatcher IPostInvocationConfiguration.CreateInvocationInspector(IPostTemplate routeToConfigure)
        {
            return new LogicalOrOfInvocations(Left.CreateInvocationInspector(routeToConfigure), Right.CreateInvocationInspector(routeToConfigure));
        }
    }
}