using System.Collections.Generic;

namespace NServiceStub
{
    /// <summary>
    /// Tracks the current step in the message sequence and buffers
    /// messages picked from the message queue for each sequence
    /// </summary>
    public class SequenceExecutionContext
    {
        private readonly Dictionary<IMessageSequence, IStep> _currentStep = new Dictionary<IMessageSequence, IStep>();
        private readonly Dictionary<IMessageSequence, Queue<object[]>> _messageBuffer = new Dictionary<IMessageSequence, Queue<object[]>>();
        private readonly IMessagePicker _picker;
        private readonly string _queue;

        public SequenceExecutionContext(IList<IMessageSequence> sequencesToExecute, string queue, IMessagePicker picker)
        {
            _queue = queue;
            _picker = picker;
            InitializeMessageBuffers(sequencesToExecute);
            InitializeCurrentSteps(sequencesToExecute);
        }

        public IMessageInitializerParameterBinder CapturedInput { get; set; }

        public object[] GetNextMessage(IMessageSequence requestor)
        {
            if (_messageBuffer[requestor].Count == 0)
            {
                if (!BufferUpANewMessage())
                    return null;
            }

            return _messageBuffer[requestor].Dequeue();
        }

        public IStep GetCurrentStep(IMessageSequence requestor)
        {
            return _currentStep[requestor];
        }

        public void SetCurrentStep(IMessageSequence requestor, IStep current)
        {
            _currentStep[requestor] = current;
        }

        private bool BufferUpANewMessage()
        {
            object[] nextMessage = _picker.PickMessage(_queue);

            if (nextMessage == null)
                return false;

            foreach (Queue<object[]> buffer in _messageBuffer.Values)
                buffer.Enqueue(nextMessage);

            return true;
        }

        private void InitializeCurrentSteps(IEnumerable<IMessageSequence> sequencesToExecute)
        {
            foreach (IMessageSequence messageSequence in sequencesToExecute)
            {
                _currentStep.Add(messageSequence, null);
            }
        }

        private void InitializeMessageBuffers(IEnumerable<IMessageSequence> sequencesToExecute)
        {
            foreach (IMessageSequence messageSequence in sequencesToExecute)
            {
                _messageBuffer.Add(messageSequence, new Queue<object[]>());
            }
        }

        public void Cleanup(IMessageSequence messageSequence)
        {
            _messageBuffer.Remove(messageSequence);
            _currentStep.Remove(messageSequence);
        }
    }
}