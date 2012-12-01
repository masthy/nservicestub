namespace NServiceStub
{
    public interface IExpectation
    {
        bool Met(object[] messages);
    }
}