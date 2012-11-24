using System;

namespace NServiceStub.Configuration
{
    public class Helpers
    {
        public static Func<object, bool> PackComparatorAsFuncOfObject<T>(Func<T, bool> comparator)
        {
            return message =>
                {
                    if (!(message is T))
                        return false;

                    return comparator((T)message);
                };
        }
    }
}