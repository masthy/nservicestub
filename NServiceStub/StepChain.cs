namespace NServiceStub
{
    public class StepChain
    {
        private Node _root;

        public void ReplaceStep(IStep stepToReplace, IStep replacement)
        {
            if (_root.Step == stepToReplace)
            {
                var tmp = new Node(replacement) { Next = _root.Next };
                _root = tmp;
            }
            else
            {
                Node currentStep = _root;

                while (currentStep.Next.Step != stepToReplace)
                    currentStep = currentStep.Next;

                var tmp = new Node(replacement) { Next = currentStep.Next.Next };
                currentStep.Next = tmp;
            }
        }

        public void SetNextStep(IStep nextStep)
        {
            if (_root == null)
            {
                _root = new Node(nextStep);
                return;
            }

            Node currentStep = _root;

            while (currentStep.Next != null)
            {
                currentStep = currentStep.Next;
            }

            currentStep.Next = new Node(nextStep);
        }

        public IStep Root
        {
            get
            {
                if (_root == null)
                    return null;

                return _root.Step;
            }
        }

        public IStep GetStepAfter(IStep step)
        {
            Node iterator = _root;

            while (iterator.Step != step)
            {
                iterator = iterator.Next;
            }

            if (iterator.Next == null)
                return null;

            return iterator.Next.Step;
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