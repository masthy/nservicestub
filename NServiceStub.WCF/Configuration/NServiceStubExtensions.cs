using System.Linq;

namespace NServiceStub.WCF.Configuration
{
    public static class NServiceStubExtensions
    {
         public static WcfProxy<T> EndPoint<T>(this ServiceStub stub, string httpEndpoint) where T : class
         {
             IWcfProxyFactory wcfProxyFactory = stub.Extensions.OfType<IWcfProxyFactory>().First();

             return wcfProxyFactory.Create<T>(httpEndpoint);
         }
    }
}