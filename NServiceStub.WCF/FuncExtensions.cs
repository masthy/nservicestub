using System;

namespace NServiceStub.WCF
{
    public static class FuncExtensions
    {
         public static Func<object> WrapInUntypedFunc<T>(this Func<T> func)
         {
             Func<object> objectFunc = () => func();

             return objectFunc;
         }
    }
}