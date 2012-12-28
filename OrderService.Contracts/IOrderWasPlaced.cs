using NServiceBus;

namespace OrderService.Contracts
{
    public interface IOrderWasPlaced : IEvent
    {
        string OrderedProduct { get; set; }

        int OrderNumber { get; set; }
    }
}