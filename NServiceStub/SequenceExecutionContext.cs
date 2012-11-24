using System.Collections.Generic;

namespace NServiceStub
{
    public class SequenceExecutionContext
    {
        private readonly Dictionary<MessageSequence, IStep> _currentStep = new Dictionary<MessageSequence, IStep>();
        private readonly Dictionary<MessageSequence, Queue<object[]>> _messageBuffer = new Dictionary<MessageSequence, Queue<object[]>>();
        private readonly IMessagePicker _picker;
        private readonly string _queue;

        public SequenceExecutionContext(IEnumerable<MessageSequence> sequencesToExecute, string queue, IMessagePicker picker)
        {
            _queue = queue;
            _picker = picker;
            InitializeMessageBuffers(sequencesToExecute);
            InitializeCurrentSteps(sequencesToExecute);
        }

        public object[] GetNextMessage(MessageSequence requestor)
        {
            if (_messageBuffer[requestor].Count == 0)
                BufferUpANewMessage();

            return _messageBuffer[requestor].Dequeue();
        }

        public IStep GetCurrentStep(MessageSequence requestor)
        {
            return _currentStep[requestor];
        }

        public void SetCurrentStep(MessageSequence requestor, IStep current)
        {
            _currentStep[requestor] = current;
        }

        private void BufferUpANewMessage()
        {
            object[] nextMessage = _picker.PickMessage(_queue);

            foreach (Queue<object[]> buffer in _messageBuffer.Values)
                buffer.Enqueue(nextMessage);
        }

        private void InitializeCurrentSteps(IEnumerable<MessageSequence> sequencesToExecute)
        {
            foreach (MessageSequence messageSequence in sequencesToExecute)
            {
                _currentStep.Add(messageSequence, null);
            }
        }

        private void InitializeMessageBuffers(IEnumerable<MessageSequence> sequencesToExecute)
        {
            foreach (MessageSequence messageSequence in sequencesToExecute)
            {
                _messageBuffer.Add(messageSequence, new Queue<object[]>());
            }
        }

        public void Cleanup(MessageSequence messageSequence)
        {
            _messageBuffer.Remove(messageSequence);
            _currentStep.Remove(messageSequence);
        }
    }
}