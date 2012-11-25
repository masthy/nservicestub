using NServiceBus;
using OrderService.Contracts;

namespace OrderService
{
    public class PlaceAnOrder : IHandleMessages<IPlaceAnOrder>
    {
        private readonly IBus _bus;

        public PlaceAnOrder(IBus bus)
        {
            _bus = bus;
        }

        public void Handle(IPlaceAnOrder message)
        {
            _bus.Publish<IOrderWasPlaced>(msg => { msg.OrderedProduct = message.Product; });
        }

    }
}