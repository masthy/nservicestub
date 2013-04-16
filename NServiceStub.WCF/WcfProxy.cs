using System;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel;
using System.ServiceModel.Description;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators;
using NServiceStub.WCF.Configuration;

namespace NServiceStub.WCF
{
    public class WcfProxy<T> : IDisposable, IWcfProxy where T : class
    {
        private readonly string _httpEndpoint;
        private readonly ServiceStub _service;
        private readonly T _serviceProxy;
        private readonly WcfCallsInterceptor _serviceImplementation = new WcfCallsInterceptor();
        private readonly ServiceHostBase _host;
        private readonly IExpressionTreeParser _parser;

        public WcfProxy(string httpEndpoint, ServiceStub service, IExpressionTreeParser parser)
        {
            _httpEndpoint = httpEndpoint;
            _service = service;
            _parser = parser;
            _serviceProxy = CreateServiceImpl(_serviceImplementation);
            _host = HostService(_serviceProxy);
        }

        public T Fallback { set { _serviceImplementation.Fallback = value; } }

        private ServiceHostBase HostService(T service)
        {
            var host = new ServiceHost(service, new Uri[0]);

            var behaviour = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            behaviour.InstanceContextMode = InstanceContextMode.Single;
            behaviour.IncludeExceptionDetailInFaults = true;

            ServiceEndpoint serviceEndpoint = host.AddServiceEndpoint(typeof(T), new BasicHttpBinding(), _httpEndpoint);

            if (!host.Description.Behaviors.Any(x => x is ServiceMetadataBehavior))
            {
                string uriString = serviceEndpoint.Address.Uri.ToString().Replace("localhost", Environment.MachineName);

                var metadataBehavior = new ServiceMetadataBehavior
                {
                    HttpGetEnabled = true,
                    HttpGetUrl = new Uri(uriString)
                };
                host.Description.Behaviors.Add(metadataBehavior);
            }


            host.Open();
            return host;
        }

        private static T CreateServiceImpl(IInterceptor interceptor)
        {
            var generator = new ProxyGenerator();
            AttributesToAvoidReplicating.Add<ServiceContractAttribute>();
            AttributesToAvoidReplicating.Add<OperationContractAttribute>();

            object proxy = generator.CreateInterfaceProxyWithoutTarget<T>(interceptor);
            return (T)proxy;
        }

        public SendAfterEndpointEventConfiguration Setup(Expression<Action<T>> methodSignatureExpectation)
        {
            IInvocationMatcher invocationMatcher = _parser.Parse(methodSignatureExpectation);

            var sequence = new TriggeredMessageSequence();
            var trigger = new InvocationTriggeringSequenceOfEvents(invocationMatcher, sequence);

            _serviceImplementation.AddInvocation(trigger, new ProduceNullReturnValue());

            return new SendAfterEndpointEventConfiguration(sequence, _service);
        }

        public MethodReturnsSetup<R> Setup<R>(Expression<Func<T, R>> methodSignatureExpectation)
        {
            IInvocationMatcher invocationMatcher = _parser.Parse(methodSignatureExpectation);

            return new MethodReturnsSetup<R>(this, _service, invocationMatcher, _parser.GetInvokedMethod(methodSignatureExpectation));
        }

        public void Dispose()
        {
            _host.Close();
        }

        void IWcfProxy.AddInvocation(IInvocationMatcher matcher, IInvocationReturnValueProducer returnValueProducer)
        {
            _serviceImplementation.AddInvocation(matcher, returnValueProducer);
        }
    }
}