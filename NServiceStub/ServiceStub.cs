using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NServiceStub.Configuration;

namespace NServiceStub
{
    public class ServiceStub : IDisposable
    {
        private readonly IIExtensionBoundToStubLifecycleFactory _extensionsFactory;
        private IFactory<IMessagePicker> _messagePickerFactory;

        private Task _runningTask;
        private IFactory<IMessageStuffer> _stufferFactory;

        public ServiceStub(string queueName, IFactory<IMessageStuffer> stufferFactory, IFactory<IMessagePicker> messagePickerFactory, IIExtensionBoundToStubLifecycleFactory extensionsFactory)
        {
            _stufferFactory = stufferFactory;
            _messagePickerFactory = messagePickerFactory;
            _extensionsFactory = extensionsFactory;
            Queue = queueName;
            MessageStuffer = _stufferFactory.Create();
            MessagePicker = _messagePickerFactory.Create();
            Sequences = new List<IMessageSequence>();
            Extensions = _extensionsFactory.Resolve();
        }

        public void AddSequence(IMessageSequence sequence)
        {
            Sequences.Add(sequence);
        }

        public void RequestStop()
        {
            RequestedStop = true;
        }

        public MessageSequenceConfiguration Setup()
        {
            return new MessageSequenceConfiguration(this);
        }

        public void Start()
        {
            if (_runningTask != null)
                throw new InvalidOperationException("The service stub has already been started");

            _runningTask = new Task(RunInternal);

            _runningTask.Start();
        }

        public IExtensionBoundToStubLifecycle[] Extensions { get; set; }

        public bool IsRunning
        {
            get { return _runningTask != null && !(_runningTask.IsCompleted || _runningTask.IsCanceled || _runningTask.IsFaulted); }
        }

        public IMessagePicker MessagePicker { get; set; }

        public IMessageStuffer MessageStuffer { get; set; }

        public string Queue { get; set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected bool RequestedStop { get; set; }
        private IList<IMessageSequence> Sequences { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            _messagePickerFactory.Release(MessagePicker);
            _stufferFactory.Release(MessageStuffer);

            foreach (var extensionBoundToStubLifecycle in Extensions)
            {
                _extensionsFactory.Release(extensionBoundToStubLifecycle);                
            }

            if (disposing)
            {
                _stufferFactory = null;
                _messagePickerFactory = null;
                MessageStuffer = null;
                MessagePicker = null;
                Queue = null;
                Sequences = null;
                Extensions = null;
            }
        }

        private void RunInternal()
        {
            var executingSequences = new List<IMessageSequence>();
            executingSequences.AddRange(Sequences);
            var executionContext = new SequenceExecutionContext(executingSequences, Queue, MessagePicker);

            IList<IMessageSequence> doneSequences = new List<IMessageSequence>();
            while (executingSequences.Count > 0 && !RequestedStop)
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

        ~ServiceStub()
        {
            Dispose(false);
        }
    }
}