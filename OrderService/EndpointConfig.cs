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
	public class EndpointConfig : IConfigureThisEndpoint, AsA_Server
    {
	    public void Customize(BusConfiguration configuration)
	    {
            var container = new WindsorContainer();
            container.Register(Component.For<IWindsorContainer>().Instance(container));

            configuration.UseContainer<WindsorBuilder>(customizations => customizations.ExistingContainer(container));

            configuration.UseSerialization<XmlSerializer>();
            configuration.UseTransport<MsmqTransport>();

            configuration.DisableFeature<Audit>();

            configuration.UsePersistence<InMemoryPersistence>();

        }
    }
}