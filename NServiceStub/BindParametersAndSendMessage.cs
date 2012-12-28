using System;

namespace NServiceStub
{
    public class BindParametersAndSendMessage<TMsg> : IStep
    {
        private readonly Sender<TMsg> _sender;
        private SequenceExecutionContext _currentContext;

        public BindParametersAndSendMessage(IMessageStuffer stuffer, string destinationQueue, Delegate messageInitializer)
        {
            _sender = new Sender<TMsg>(stuffer, destinationQueue, WrapMessageInitializerInBinder(messageInitializer));
        }

        private Action<TMsg> WrapMessageInitializerInBinder(Delegate messageInitializer)
        {

            return msg =>
                {
                    object[] resolvedParameters = _currentContext.CapturedInput.Bind(msg, messageInitializer);
                    messageInitializer.DynamicInvoke(resolvedParameters);
                };
        }

        public void Execute(SequenceExecutionContext context)
        {
            try
            {
                _currentContext = context;
                _sender.Send();
            }
            finally
            {
                _currentContext = null;                
            }
        }
    }
}