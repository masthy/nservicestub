using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace NServiceStub.WCF.Configuration
{
    public class Bootstrapper : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IWcfProxyFactory>().AsFactory().Forward<IExtensionBoundToStubLifecycle>().LifeStyle.Transient);
            container.Register(Component.For<IExpressionTreeParser>().ImplementedBy<ExpressionTreeParser>());
            container.Register(Component.For(typeof(WcfProxy<>)).LifeStyle.Scoped<OneProxyPrStubScopeAccessor>());
        }
    }
}