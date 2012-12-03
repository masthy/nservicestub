using Castle.Windsor.Installer;
using NServiceStub.Configuration;

namespace NServiceStub.WCF.Configuration
{
    public static class SConfiguration
    {
        public static StubConfiguration WcfEndPoints(this StubConfiguration configuration)
        {
            configuration.Container.Install(FromAssembly.This());
            return configuration;
        }
    }
}