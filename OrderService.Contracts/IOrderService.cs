using System.ServiceModel;

namespace OrderService.Contracts
{
    [ServiceKnownType("GetCommandTypes", typeof(CommandRegistry))]
    [ServiceContract]
    public interface IOrderService
    {
        [OperationContract]
        bool PlaceOrder(string product);

        [OperationContract]
        void CancelOrder(string product);

        [OperationContract]
        Date WhenWasOrderLastPlaced();

        [OperationContract]
        void ExecuteCommand(Command command);
    }
}