using System.Runtime.InteropServices;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Settings;
using NServiceBus.Unicast;

namespace NServiceStub.NServiceBus
{
    public class InternalBusCreator
    {
        public static UnicastBus CreateBus()
        {
            SettingsHolder.Reset();
            Configure.Serialization.Xml();
            Configure.Features.Disable<SecondLevelRetries>();
            Configure.Features.Disable<Audit>();
            Configure.Transactions.Disable();
            Configure.Transactions.Advanced(s => s.DisableDistributedTransactions());

            IBus bus = Configure.With().DefineEndpointName("nservicestub")
                .Log4Net()
                .DefaultBuilder()
                .UseTransport<Msmq>()
                .PurgeOnStartup(false)
                .UnicastBus()
                .ImpersonateSender(false)
                .SendOnly();

            return bus as UnicastBus;
        }
    }
}