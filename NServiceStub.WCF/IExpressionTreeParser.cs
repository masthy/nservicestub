using System;
using System.Linq.Expressions;
using System.Reflection;

namespace NServiceStub.WCF
{
    public interface IExpressionTreeParser
    {
        IInvocationMatcher Parse<T, R>(Expression<Func<T, R>> methodSignatureExpectation);

        IInvocationMatcher Parse<T>(Expression<Action<T>> methodSignatureExpectation);

        MethodInfo GetInvokedMethod<T,R>(Expression<Func<T, R>> methodSignatureExpectation);

        MethodInfo GetInvokedMethod<T>(Expression<Action<T>> methodSignatureExpectation);

    }
}