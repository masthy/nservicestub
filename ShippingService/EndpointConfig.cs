using NServiceBus;
using NServiceBus.Features;

namespace ShippingService 
{
    /*
		This class configures this endpoint as a Server. More information about how to configure the NServiceBus host
		can be found here: http://nservicebus.com/GenericHost.aspx
	*/
	public class EndpointConfig : IConfigureThisEndpoint, AsA_Server
    {
        public void Customize(BusConfiguration configuration)
        {
            configuration.UseContainer<WindsorBuilder>();

            configuration.UseSerialization<XmlSerializer>();
            configuration.UseTransport<MsmqTransport>();

            configuration.DisableFeature<Audit>();

            configuration.UsePersistence<InMemoryPersistence>();
        }
    }
}