using System.Linq;

namespace NServiceStub.WCF.Configuration
{
    public static class NServiceStubExtensions
    {
         public static WcfProxy<T> WcfEndPoint<T>(this ServiceStub stub, string httpEndpoint) where T : class
         {
             IWcfProxyFactory wcfProxyFactory = stub.Extensions.OfType<IWcfProxyFactory>().First();

             return wcfProxyFactory.Create<T>(httpEndpoint, stub);
         }

        public static WcfProxy<T> WcfEndPoint<T>(this ServiceStub stub, string httpEndpoint, T fallback) where T : class
        {
            WcfProxy<T> proxy = stub.WcfEndPoint<T>(httpEndpoint);
            proxy.Fallback = fallback;
            return proxy;
        }
    }
}