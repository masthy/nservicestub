namespace OrderService.Contracts
{
    public class DeleteOrder : Command
    {
        public int OrderNumber { get; set; }
    }
}