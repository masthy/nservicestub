using System.Linq;

namespace NServiceStub.WCF.Configuration
{
    public static class NServiceStubExtensions
    {
         public static WcfProxy<T> WcfEndPoint<T>(this ServiceStub stub, string endpoint) where T : class
         {
             IWcfProxyFactory wcfProxyFactory = stub.Extensions.OfType<IWcfProxyFactory>().First();

             return wcfProxyFactory.Create<T>(endpoint, stub);
         }

        public static WcfProxy<T> WcfEndPoint<T>(this ServiceStub stub, string endpoint, T fallback) where T : class
        {
            WcfProxy<T> proxy = stub.WcfEndPoint<T>(endpoint);
            proxy.Fallback = fallback;
            return proxy;
        }
    }
}