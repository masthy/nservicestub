using System;
using System.Linq;

namespace NServiceStub.WCF.Configuration
{
    public static class NServiceStubExtensions
    {
        public static WcfProxy<T> WcfEndPoint<T>(this ServiceStub stub, string endpoint) where T : class
        {
            IWcfProxyFactory wcfProxyFactory = stub.Extensions.OfType<IWcfProxyFactory>().FirstOrDefault();

            if (wcfProxyFactory == null)
                throw new InvalidOperationException("Did you forget to configure the stub with wcf endpoint extension (.WcfEndPoints)?");

            return wcfProxyFactory.Create<T>(endpoint, stub);
        }

        /// <summary>
        /// Specify an endpoint which is configured in the .config file
        /// </summary>
        /// <example>
        /// Service contract name: Acme.IShippingService service name in app.config: Acme.IShippingServiceStub
        /// </example>
        public static WcfProxy<T> WcfEndPoint<T>(this ServiceStub stub) where T : class
        {
            return stub.WcfEndPoint<T>(String.Empty);
        }

        public static WcfProxy<T> WcfEndPoint<T>(this ServiceStub stub, string endpoint, T fallback) where T : class
        {
            WcfProxy<T> proxy = stub.WcfEndPoint<T>(endpoint);
            proxy.Fallback = fallback;
            return proxy;
        }
    }
}