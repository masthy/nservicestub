using System;

namespace NServiceStub
{
    public class MessageSequence
    {
        private Node _steps;

        public void ExecuteNextStep(SequenceExecutionContext executionContext)
        {
            if (Done)
                throw new InvalidOperationException("Sequence is done, nothing more to do");

            if (UpdateExecutionContextWithNextStep(executionContext))
            {
                IStep currentStep = executionContext.GetCurrentStep(this);
                currentStep.Execute(executionContext);
            }
        }

        public void ReplaceStep(IStep stepToReplace, IStep replacement)
        {
            if (_steps.Step == stepToReplace)
            {
                var tmp = new Node(replacement) {Next = _steps.Next};
                _steps = tmp;
            }
            else
            {
                Node currentStep = _steps;

                while (currentStep.Next.Step != stepToReplace)
                    currentStep = currentStep.Next;

                var tmp = new Node(replacement) {Next = currentStep.Next};
                currentStep.Next = tmp;
            }
        }

        public void SetNextStep(IStep nextStep)
        {
            if (_steps == null)
            {
                _steps = new Node(nextStep);
                return;
            }

            Node currentStep = _steps;

            while (currentStep.Next != null)
            {
                currentStep = currentStep.Next;
            }

            currentStep.Next = new Node(nextStep);
        }

        public bool Done { get; set; }

        private Node GetStepAfter(IStep current)
        {
            Node iterator = _steps;

            while (iterator.Step != current)
            {
                iterator = iterator.Next;
            }
            return iterator.Next;
        }

        private bool UpdateExecutionContextWithNextStep(SequenceExecutionContext executionContext)
        {
            if (executionContext.GetCurrentStep(this) == null)
                executionContext.SetCurrentStep(this, _steps.Step);
            else
            {
                IStep current = executionContext.GetCurrentStep(this);
                Node next = GetStepAfter(current);

                if (next == null)
                {
                    executionContext.Cleanup(this);
                    Done = true;
                }
                else
                    executionContext.SetCurrentStep(this, next.Step);
            }

            return !Done;
        }

        private class Node
        {
            public Node(IStep step)
            {
                Step = step;
            }

            public Node Next { get; set; }

            public IStep Step { get; private set; }
        }
    }
}