namespace NServiceStub
{
    public interface INonRepeatingMessageSequence : IMessageSequence
    {
        bool Done { get; set; }         
    }
}