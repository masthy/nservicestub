namespace NServiceStub
{
    public class SendMessage : IStep
    {
        private readonly ISender _sender;

        public SendMessage(ISender sender)
        {
            _sender = sender;
        }

        public void Execute(SequenceExecutionContext context)
        {
            _sender.Send();
        }
    }
}