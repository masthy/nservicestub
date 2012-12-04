using System.ServiceModel;

namespace OrderService.Contracts
{
    [ServiceContract]
    public interface IOrderService
    {
        [OperationContract]
        bool PlaceOrder(string product);

        [OperationContract]
        void CancelOrder(string product);
    }
}