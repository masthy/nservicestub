using System.Linq;

namespace NServiceStub.Rest.Configuration
{
    public static class NServiceStubExtensions
    {
         public static RestApi RestEndpoint(this ServiceStub stub, string baseUrl)
         {
             IRestApiFactory factory = stub.Extensions.OfType<IRestApiFactory>().First();

             return factory.Create(baseUrl, stub);
         }
    }
}