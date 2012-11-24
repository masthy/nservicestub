using Castle.Windsor.Installer;
using NServiceStub.Configuration;

namespace NServiceStub.NServiceBus
{
    public static class Configuration
    {
        public static StubConfiguration NServiceBusSerializers(this StubConfiguration configuration)
        {
            configuration.Container.Install(FromAssembly.This());
            return configuration;
        }
    }
}