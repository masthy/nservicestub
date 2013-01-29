namespace NServiceStub.Rest
{
    public interface IGetTemplate<R> : IGetTemplate
    {
        void AddReturn(IInvocationMatcher invocation, IInvocationReturnValueProducer<R> returnValue);
    }

    public interface IGetTemplate : IRouteTemplate
    {
        Get Route { get; }        
    }
}