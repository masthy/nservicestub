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
        bool AreYouHappy();

        [OperationContract]
        void ExecuteCommand(Command command);
    }
}