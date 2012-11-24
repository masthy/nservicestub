namespace NServiceStub
{
    public interface IExpectation
    {
        bool Met(object[] messages);
        ISender Sender { get; set; }
    }
}