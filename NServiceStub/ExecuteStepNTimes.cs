namespace NServiceStub
{
    public class ExecuteStepNTimes : IStep
    {
        private readonly IStep _stepToRepeat;
        private readonly int _numberOfSends;

        public ExecuteStepNTimes(IStep stepToRepeat, int numberOfSends)
        {
            _stepToRepeat = stepToRepeat;
            _numberOfSends = numberOfSends;
        }

        public void Execute(SequenceExecutionContext context)
        {
            for(int i = 0; i < _numberOfSends; i++)
                _stepToRepeat.Execute(context);
        }
    }
}