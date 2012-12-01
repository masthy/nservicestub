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
    }
}