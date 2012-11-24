namespace NServiceStub
{
    public interface IMessagePicker
    {
        object[] PickMessage(string fromQueue);
    }
}