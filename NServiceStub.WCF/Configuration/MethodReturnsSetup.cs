﻿using System;
using System.Reflection;

namespace NServiceStub.WCF.Configuration
{
    public class MethodReturnsSetup<R>
    {
        private readonly IWcfProxy _wcfProxy;
        private readonly ServiceStub _service;
        private readonly IInvocationMatcher _invocationMatcher;
        private readonly MethodInfo _serviceMethod;

        public MethodReturnsSetup(IWcfProxy wcfProxy, ServiceStub service, IInvocationMatcher invocationMatcher, MethodInfo serviceMethod)
        {
            _wcfProxy = wcfProxy;
            _service = service;
            _invocationMatcher = invocationMatcher;
            _serviceMethod = serviceMethod;
        }

        public SendAfterEndpointEventConfiguration Returns(Func<R> result)
        {
            return ReturnsInternal(new ProduceDelegateReturnValue(result, _serviceMethod));
        }

        public SendAfterEndpointEventConfiguration Returns<T>(Func<T,R> result)
        {
            return ReturnsInternal(new ProduceDelegateReturnValue(result, _serviceMethod));
        }

        public SendAfterEndpointEventConfiguration Returns<T1, T2>(Func<T1, T2, R> result)
        {
            return ReturnsInternal(new ProduceDelegateReturnValue(result, _serviceMethod));
        }

        public SendAfterEndpointEventConfiguration Returns<T1, T2, T3>(Func<T1, T2, T3, R> result)
        {
            return ReturnsInternal(new ProduceDelegateReturnValue(result, _serviceMethod));
        }

        public SendAfterEndpointEventConfiguration Returns<T1, T2, T3, T4>(Func<T1, T2, T3, T4, R> result)
        {
            return ReturnsInternal(new ProduceDelegateReturnValue(result, _serviceMethod));
        }

        public SendAfterEndpointEventConfiguration Returns<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, R> result)
        {
            return ReturnsInternal(new ProduceDelegateReturnValue(result, _serviceMethod));
        }

        public SendAfterEndpointEventConfiguration Returns<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, R> result)
        {
            return ReturnsInternal(new ProduceDelegateReturnValue(result, _serviceMethod));
        }

        public SendAfterEndpointEventConfiguration Returns<T1, T2, T3, T4, T5, T6, T7>(Func<T1, T2, T3, T4, T5, T6, T7, R> result)
        {
            return ReturnsInternal(new ProduceDelegateReturnValue(result, _serviceMethod));
        }

        public SendAfterEndpointEventConfiguration Returns<T1, T2, T3, T4, T5, T6, T7, T8>(Func<T1, T2, T3, T4, T5, T6, T7, T8, R> result)
        {
            return ReturnsInternal(new ProduceDelegateReturnValue(result, _serviceMethod));
        }

        public SendAfterEndpointEventConfiguration Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, R> result)
        {
            return ReturnsInternal(new ProduceDelegateReturnValue(result, _serviceMethod));
        }

        public SendAfterEndpointEventConfiguration Returns<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, R> result)
        {
            return ReturnsInternal(new ProduceDelegateReturnValue(result, _serviceMethod));
        }

        private SendAfterEndpointEventConfiguration ReturnsInternal(IInvocationReturnValueProducer returnValueProducer)
        {
            var sequence = new TriggeredMessageSequence();
            var trigger = new InvocationTriggeringSequenceOfEvents(_invocationMatcher, sequence);

            _wcfProxy.AddInvocation(trigger, returnValueProducer);

            return new SendAfterEndpointEventConfiguration(sequence, _service);
        }
    }
}