using Castle.Windsor.Installer;
using NServiceStub.Configuration;

namespace NServiceStub.Rest.Configuration
{
    public static class BootstrapperConfiguration
    {
         public static StubConfiguration Restful(this StubConfiguration configuration)
         {
             configuration.Container.Install(FromAssembly.This());
             return configuration;
         }
    }
}