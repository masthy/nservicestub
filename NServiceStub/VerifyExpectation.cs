namespace NServiceStub
{
    public class VerifyExpectation : IStep
    {
        private readonly IMessageSequence _owningSequence;
        private readonly IExpectation _expectation;

        public VerifyExpectation(IMessageSequence owningSequence, IExpectation expectation)
        {            
            _owningSequence = owningSequence;
            _expectation = expectation;
        }

        public void Execute(SequenceExecutionContext context)
        {
            bool expectationHasBeenMet = false;

            while (!expectationHasBeenMet)
            {
                expectationHasBeenMet = _expectation.Met(context.GetNextMessage(_owningSequence));
            }
        }
    }
}