namespace NServiceStub.Rest
{
    public interface IRouteTemplate<R> : IRouteTemplate
    {
    }

    public interface IRouteTemplate
    {
        bool Matches(RequestWrapper request);

        bool TryInvocation(RequestWrapper request, out object returnValue);

        void AddReturn(IInvocationMatcher invocation, IInvocationReturnValueProducer returnValue);

        Route Route { get; }                
    }
}