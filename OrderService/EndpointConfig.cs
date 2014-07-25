using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NServiceBus;
using NServiceBus.Features;

namespace OrderService 
{
    /*
		This class configures this endpoint as a Server. More information about how to configure the NServiceBus host
		can be found here: http://nservicebus.com/GenericHost.aspx
	*/
	public class EndpointConfig : IConfigureThisEndpoint, IWantCustomInitialization, AsA_Server
    {
        public void Init()
        {
            var container = new WindsorContainer();
            container.Register(Component.For<IWindsorContainer>().Instance(container));

            Configure.Features.Disable<Audit>();
            Configure.Serialization.Xml();
            Configure.With()
                .CastleWindsorBuilder(container)
                .UseTransport<Msmq>()
                .RavenSubscriptionStorage()
                .UnicastBus()
                .CreateBus()
                .Start();
        }
    }
}