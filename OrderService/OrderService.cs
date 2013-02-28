using OrderService.Contracts;

namespace OrderService
{
    public class OrderService : IOrderService
    {
        public bool PlaceOrder(string product)
        {
            return true;
        }

        public void CancelOrder(string product)
        {
            
        }

        public Date WhenWasOrderLastPlaced()
        {
            throw new System.NotImplementedException();
        }

        public bool AreYouHappy()
        {
            throw new System.NotImplementedException();
        }

        public OrderStatus GetStatus()
        {
            throw new System.NotImplementedException();
        }

        public void ExecuteCommand(Command command)
        {
            
        }
    }
}