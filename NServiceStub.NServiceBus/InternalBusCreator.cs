using NServiceBus;
using NServiceBus.Unicast;

namespace NServiceStub.NServiceBus
{
    public class InternalBusCreator
    {
        public static UnicastBus CreateBus()
        {
            IBus bus = Configure.With().DefineEndpointName("nservicestub")
                .Log4Net()
                .DefaultBuilder()
                .XmlSerializer()
                .MsmqTransport()
                .DisableSecondLevelRetries()
                .IsTransactional(false)
                .PurgeOnStartup(false)
                .UnicastBus()
                .ImpersonateSender(false)
                .SendOnly();

            return bus as UnicastBus;
        }
    }
}