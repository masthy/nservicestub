using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Unicast;

namespace NServiceStub.NServiceBus
{
    public static class InternalBusCreator
    {
        public static UnicastBus CreateBus()
        {
            var configuration = new BusConfiguration();

            configuration.UseSerialization<XmlSerializer>();
            configuration.DisableFeature<SecondLevelRetries>();
            configuration.DisableFeature<Audit>();
            configuration.UsePersistence<InMemoryPersistence>();
            configuration.Transactions()
                .DisableDistributedTransactions()
                .Disable();

            configuration.EndpointName("nservicestub");

            return (UnicastBus) Bus.Create(configuration).Start();
        }
    }
}