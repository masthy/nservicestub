using NServiceBus;

namespace OrderService.Contracts
{
    public interface IPlaceAnOrder : ICommand
    {
        string Product { get; set; }
    }
}