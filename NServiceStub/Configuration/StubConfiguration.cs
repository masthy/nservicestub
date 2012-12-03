using Castle.Windsor;

namespace NServiceStub.Configuration
{
    public class StubConfiguration
    {
        private readonly IWindsorContainer _container;

        internal StubConfiguration(IWindsorContainer container)
        {
            _container = container;
        }

        public ServiceStub Create(string queueName)
        {
            return new ServiceStub(queueName, _container.Resolve<IFactory<IMessageStuffer>>(), _container.Resolve<IFactory<IMessagePicker>>(), _container.Resolve<IIExtensionBoundToStubLifecycleFactory>());
        }

        public IWindsorContainer Container
        {
            get { return _container; }
        }
    }
}