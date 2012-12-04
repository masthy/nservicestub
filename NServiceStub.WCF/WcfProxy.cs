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

        private ServiceHostBase HostService(T service)
        {
            var host = new ServiceHost(service, new Uri[0]);

            var behaviour = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            behaviour.InstanceContextMode = InstanceContextMode.Single;

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

        public SendAfterWcfEventConfiguration Setup(Expression<Action<T>> methodSignatureExpectation)
        {
            IInvocationMatcher invocationMatcher = _parser.Parse(methodSignatureExpectation);

            var sequence = new WcfTriggeredMessageSequence();
            var trigger = new WcfMessageSequenceTrigger(invocationMatcher, sequence);

            _serviceImplementation.AddInvocation(trigger, () => null);

            return new SendAfterWcfEventConfiguration(sequence, _service);
        }

        public MethodReturnsSetup<R> Setup<R>(Expression<Func<T, R>> methodSignatureExpectation)
        {
            IInvocationMatcher invocationMatcher = _parser.Parse(methodSignatureExpectation);

            return new MethodReturnsSetup<R>(this, _service, invocationMatcher);
        }

        public void Dispose()
        {
            _host.Close();
        }

        void IWcfProxy.AddInvocation(IInvocationMatcher matcher, Func<object> returnValueProducer)
        {
            _serviceImplementation.AddInvocation(matcher, returnValueProducer);
        }
    }
}