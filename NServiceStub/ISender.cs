namespace NServiceStub
{
    public interface ISender
    {
        void Send();

        IExpectation Expectation { get; set; }
        ISender Sender { get; set; }
    }
}