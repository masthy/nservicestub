using System.ServiceModel;

namespace NServiceStub.IntegrationTests.WCF
{
    [ServiceContract]
    public interface ISomeServiceConfiguredInAppConfig
    {
        [OperationContract]
        void Hello();         
    }
}