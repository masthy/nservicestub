namespace NServiceStub.Rest
{
    public interface IPostTemplate : IRouteTemplate
    {
        Post Route { get; }

        void AddReturn(IInvocationMatcher invocation, IInvocationReturnValueProducer<object> returnValue);
    }
}