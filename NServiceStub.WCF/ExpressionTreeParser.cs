using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NServiceStub.WCF
{
    public class ExpressionTreeParser : IExpressionTreeParser
    {
        public IInvocationMatcher Parse<T, R>(Expression<Func<T, R>> methodSignatureExpectation)
        {
            var serviceMethod = methodSignatureExpectation.Body as MethodCallExpression;

            return ParseInternal(serviceMethod);
        }

        public IInvocationMatcher Parse<T>(Expression<Action<T>> methodSignatureExpectation)
        {
            var serviceMethod = methodSignatureExpectation.Body as MethodCallExpression;

            return ParseInternal(serviceMethod);
        }

        public MethodInfo GetInvokedMethod<T, R>(Expression<Func<T, R>> methodSignatureExpectation)
        {
            var serviceMethod = methodSignatureExpectation.Body as MethodCallExpression;
            return serviceMethod.Method;
        }

        private static IInvocationMatcher ParseInternal(MethodCallExpression serviceMethod)
        {
            var matchers = new List<Func<object, bool>>();
            foreach (Expression argument in serviceMethod.Arguments)
            {
                var parameterExpression = argument as MethodCallExpression;

                if (parameterExpression != null && parameterExpression.Method.DeclaringType == typeof(Parameter))
                {
                    if (parameterExpression.Method.Name == "Equals")
                    {
                        Expression expression = parameterExpression.Arguments[0];
                        Delegate @delegate = (expression as LambdaExpression).Compile();
                        matchers.Add(obj => (bool)@delegate.DynamicInvoke(obj));
                    }
                    else if (parameterExpression.Method.Name == "Any")
                    {
                        Type acceptedType = parameterExpression.Method.GetGenericArguments()[0];

                        if (acceptedType == typeof(object) || acceptedType == argument.Type)
                            matchers.Add(obj => true);
                        else
                        {
                            matchers.Add(obj =>
                                {
                                    if (obj == null)
                                        return true;

                                    return obj.GetType().IsAssignableFrom(acceptedType);
                                });
                        }
                    }
                    else
                    {
                        throw new NotImplementedException("Have i've been lazy?");
                    }
                }
                else
                {
                    throw new NotSupportedException("Use Parameter.Equals or Parameter.Any when specifying expected invocations");
                }
            }

            return new InvocationMatcher(matchers.ToArray());
        }
    }
}