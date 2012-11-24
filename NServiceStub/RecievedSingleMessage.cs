using System;

namespace NServiceStub
{
    public class RecievedSingleMessage : IExpectation
    {
        private readonly Func<object, bool> _msgComparator;

        public RecievedSingleMessage(Func<object, bool> msgComparator)
        {
            _msgComparator = msgComparator;
        }

        public bool Met(object[] messages)
        {
            if (messages != null && messages.Length == 1)
            {
                return _msgComparator(messages[0]);
            }
            else
            {
                return false;
            }
        }

        public ISender Sender { get; set; }

    }
}