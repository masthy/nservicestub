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
                expectationHasBeenMet = TestExpectationIfAnyMessageOnQueue(context);
            }
        }

        public bool TestExpectationIfAnyMessageOnQueue(SequenceExecutionContext context)
        {
            object[] nextMessage = context.GetNextMessage(_owningSequence);

            if (nextMessage == null)
                return false;

            return _expectation.Met(nextMessage);
        }
    }
}