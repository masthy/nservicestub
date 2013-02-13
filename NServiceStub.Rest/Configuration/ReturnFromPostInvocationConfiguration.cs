namespace NServiceStub.Rest.Configuration
{
    public class ReturnFromPostInvocationConfiguration : SendAfterEndpointEventConfiguration
    {
        private readonly TriggeredMessageSequence _sequenceBeingConfigured;
        private readonly ServiceStub _componentBeingConfigured;
        private readonly NullOrInvocationReturnValueProducer _returnValueProxy;

        public ReturnFromPostInvocationConfiguration(TriggeredMessageSequence sequenceBeingConfigured, ServiceStub componentBeingConfigured, NullOrInvocationReturnValueProducer returnValueProxy) : base(sequenceBeingConfigured, componentBeingConfigured)
        {
            _sequenceBeingConfigured = sequenceBeingConfigured;
            _componentBeingConfigured = componentBeingConfigured;
            _returnValueProxy = returnValueProxy;
        }

        public SendAfterEndpointEventConfiguration Returns<R>(R returnValue)
        {
            _returnValueProxy.NonNullReturnValue = new ProduceStaticReturnValue(returnValue);

            return new SendAfterEndpointEventConfiguration(_sequenceBeingConfigured, _componentBeingConfigured);
        }

    }
}