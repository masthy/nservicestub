using System;

namespace NServiceStub.Rest
{
    public class QueryStringParseException : Exception
    {
        public QueryStringParseException(string message) : base(message)
        {
            
        }
    }
}