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

        public MethodInfo GetInvokedMethod<T>(Expression<Action<T>> methodSignatureExpectation)
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
                        AddEqualsMatcher(parameterExpression, matchers);
                    }
                    else if (parameterExpression.Method.Name == "Any")
                    {
                        Type acceptedType = parameterExpression.Method.GetGenericArguments()[0];

                        if (acceptedType == typeof(object))
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
                        throw new NotImplementedException("Have I've been lazy?");
                    }
                }
                else
                {
                    throw new NotSupportedException("Use Parameter.Equals or Parameter.Any when specifying expected invocations");
                }
            }

            return new InvocationMatcher(matchers.ToArray(), serviceMethod.Method);
        }

        private static void AddEqualsMatcher(MethodCallExpression parameterExpression, List<Func<object, bool>> matchers)
        {
            Expression expression = parameterExpression.Arguments[0];
            Delegate @delegate = (expression as LambdaExpression).Compile();

            ParameterInfo[] parameters = @delegate.Method.GetParameters();
            if (parameters.Length == 2 && parameters[1].ParameterType.IsClass && parameters[1].ParameterType != typeof(string))
            {
                matchers.Add(obj =>
                    {
                        Type parameterType = @delegate.Method.GetParameters()[1].ParameterType;
                        if (obj != null && !parameterType.IsInstanceOfType(obj))
                            return false;

                        return (bool)@delegate.DynamicInvoke(obj);
                    });
            }
            else
                matchers.Add(obj => (bool)@delegate.DynamicInvoke(obj));
        }
    }
}