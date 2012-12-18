using System.Linq;

namespace NServiceStub.Rest.Configuration
{
    public static class NServiceStubExtensions
    {
         public static RestApi RestEndpoint(this ServiceStub stub, string baseUrl)
         {
             IRestStubFactory factory = stub.Extensions.OfType<IRestStubFactory>().First();

             return factory.Create(baseUrl, stub);
         }
    }
}