using System;
using System.Linq.Expressions;

namespace NServiceStub.WCF
{
    public interface IExpressionTreeParser
    {
        IInvocationMatcher Parse<T, R>(Expression<Func<T, R>> methodSignatureExpectation);

        IInvocationMatcher Parse<T>(Expression<Action<T>> methodSignatureExpectation);

    }
}