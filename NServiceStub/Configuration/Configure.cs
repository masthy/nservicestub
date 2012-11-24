using Castle.Facilities.TypedFactory;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace NServiceStub.Configuration
{
    public static class Configure
    {
        public static StubConfiguration Stub()
        {
            var windsorContainer = new WindsorContainer();
            windsorContainer.AddFacility<TypedFactoryFacility>();
            windsorContainer.Install(FromAssembly.This());

            return new StubConfiguration(windsorContainer);
        }
    }
}