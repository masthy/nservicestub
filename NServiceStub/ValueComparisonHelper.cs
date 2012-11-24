using System;

namespace NServiceStub
{
    public class ValueComparisonHelper
    {
        public static bool Compare(object msg, object expectedMsg)
        {
            return true;
        }

        public static Func<object, bool> CreateComparisonDelegate<T>(T expectedMsg) where T : class
        {
            return msg =>
                {
                    if (!(msg is T))
                        return false;

                    return Compare(msg, expectedMsg);
                };
        }
    }
}