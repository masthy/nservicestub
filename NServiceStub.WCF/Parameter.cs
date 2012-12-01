using System;

namespace NServiceStub.WCF
{
    public static class Parameter
    {
        public static T Any<T>()
        {
            throw new NotImplementedException("Theese are just templates");
        }

        public static T Equals<T>(Func<T, bool> parameterMatch)
        {
            throw new NotImplementedException("Theese are just templates");            
        }
    }
}