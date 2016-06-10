using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NServiceBus;
using NServiceBus.Unicast;

namespace NServiceStub.NServiceBus
{
    public class Bootstrapper : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly().Where(type => type.GetInterfaces().Length > 0).WithService.AllInterfaces());
            container.Register(Component.For<UnicastBus>().UsingFactoryMethod(kernel => InternalBusCreator.CreateBus()));
        }
    }
}