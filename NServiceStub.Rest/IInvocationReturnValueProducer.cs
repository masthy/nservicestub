namespace NServiceStub.Rest
{
    public interface IInvocationReturnValueProducer<R>
    {
        R Produce(string rawUrl, IRouteDefinition route);
    }
}