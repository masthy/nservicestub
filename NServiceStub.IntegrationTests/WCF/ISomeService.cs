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

        [OperationContract]
        FourInputParamsReturnValue IHave4InputParameters(string paramOne, string paramTwo, bool paramThree, string paramFour);
    }

    public class FourInputParamsReturnValue
    {
        public string ReturnOne { get; set; }

        public string ReturnTwo { get; set; }

        public bool ReturnThree { get; set; }

        public string ReturnFour { get; set; }
    }
}