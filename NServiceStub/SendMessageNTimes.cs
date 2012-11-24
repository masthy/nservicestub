namespace NServiceStub
{
    public class SendMessageNTimes : IStep
    {
        private readonly SendMessage _sendStepToRepeat;
        private readonly int _numberOfSends;

        public SendMessageNTimes(SendMessage sendStepToRepeatToRepeat, int numberOfSends)
        {
            _sendStepToRepeat = sendStepToRepeatToRepeat;
            _numberOfSends = numberOfSends;
        }

        public void Execute(SequenceExecutionContext context)
        {
            for(int i = 0; i < _numberOfSends; i++)
                _sendStepToRepeat.Execute(context);
        }
    }
}