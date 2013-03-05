using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace NServiceStub.Rest.Configuration
{
    public class Bootstrapper : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IRestApiFactory>().AsFactory().Forward<IExtensionBoundToStubLifecycle>().LifeStyle.Transient);
            container.Register(Component.For<QueryStringParser>());
            container.Register(Component.For<RestApi>().LifeStyle.Transient);
        }
    }
}