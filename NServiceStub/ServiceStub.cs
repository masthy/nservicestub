using System;
using System.Collections.Generic;
using NServiceStub.Configuration;

namespace NServiceStub
{
    public class ServiceStub : IDisposable
    {
        private IFactory<IMessagePicker> _messagePickerFactory;
        private IFactory<IMessageStuffer> _stufferFactory;

        public ServiceStub(string queueName, IFactory<IMessageStuffer> stufferFactory, IFactory<IMessagePicker> messagePickerFactory)
        {
            _stufferFactory = stufferFactory;
            _messagePickerFactory = messagePickerFactory;
            Queue = queueName;
            MessageStuffer = _stufferFactory.Create();
            MessagePicker = _messagePickerFactory.Create();
            Sequences = new List<IMessageSequence>();
        }

        public void AddSequence(IMessageSequence sequence)
        {
            Sequences.Add(sequence);
        }

        public void Begin()
        {
            var executingSequences = new List<IMessageSequence>();
            executingSequences.AddRange(Sequences);
            var executionContext = new SequenceExecutionContext(executingSequences, Queue, MessagePicker);

            IList<IMessageSequence> doneSequences = new List<IMessageSequence>();
            while (executingSequences.Count > 0)
            {
                doneSequences.Clear();

                foreach (IMessageSequence sequence in executingSequences)
                {
                    sequence.ExecuteNextStep(executionContext);

                    if (sequence.Done)
                        doneSequences.Add(sequence);
                }

                foreach (IMessageSequence messageSequence in doneSequences)
                {
                    executingSequences.Remove(messageSequence);
                }
            }
        }

        public MessageSequenceConfiguration Configure()
        {
            var messageSequence = new MessageSequence();
            Sequences.Add(messageSequence);

            return new MessageSequenceConfiguration(this, messageSequence);
        }

        public IMessagePicker MessagePicker { get; set; }

        public IMessageStuffer MessageStuffer { get; set; }

        public string Queue { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private IList<IMessageSequence> Sequences { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            _messagePickerFactory.Release(MessagePicker);
            _stufferFactory.Release(MessageStuffer);

            if (disposing)
            {
                _stufferFactory = null;
                _messagePickerFactory = null;
                MessageStuffer = null;
                MessagePicker = null;
                Queue = null;
                Sequences = null;
            }
        }

        ~ServiceStub()
        {
            Dispose(false);
        }
    }
}