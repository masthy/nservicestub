using System.ServiceModel;

namespace NServiceStub.IntegrationTests.WCF
{
    [ServiceContract]
    public interface ISomeService
    {
        [OperationContract]
        string PingBack(string stringToPingBack);

        [OperationContract]
        string IHaveMultipleInputParameters(string paramOne, string paramTwo, bool paramThree);

    }
}