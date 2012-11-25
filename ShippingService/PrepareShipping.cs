using NServiceBus;
using OrderService.Contracts;

namespace ShippingService
{
    public class PrepareShipping : IHandleMessages<IOrderWasPlaced>
    {
        public void Handle(IOrderWasPlaced message)
        {
        }
    }
}