using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace NServiceStub.Configuration
{
    public class CastleInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For(typeof(IFactory<>)).AsFactory());
            container.Register(Component.For<IIExtensionBoundToStubLifecycleFactory>().AsFactory());
        }
    }
}