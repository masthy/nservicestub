using System;

namespace NServiceStub.WCF.Configuration
{
    public class MethodReturnsSetup<R>
    {
        private readonly IWcfProxy _wcfProxy;
        private readonly IInvocationMatcher _invocationMatcher;

        public MethodReturnsSetup(IWcfProxy wcfProxy, IInvocationMatcher invocationMatcher)
        {
            _wcfProxy = wcfProxy;
            _invocationMatcher = invocationMatcher;
        }

        public SendAfterWcfEventConfiguration Returns(Func<R> result)
        {
            var sequence = new WcfTriggeredMessageSequence();
            var trigger = new WcfMessageSequenceTrigger(_invocationMatcher, sequence);

            _wcfProxy.AddInvocation(trigger, result.WrapInUntypedFunc());

            return new SendAfterWcfEventConfiguration(sequence);
        }
        
    }
}