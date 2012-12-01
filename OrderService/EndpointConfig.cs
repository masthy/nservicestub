using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NServiceBus;

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

            Configure.With()
                .CastleWindsorBuilder(container)
                .XmlSerializer()
                .MsmqTransport()
                .RavenSubscriptionStorage()
                .UnicastBus()
                .CreateBus()
                .Start();
        }
    }
}