using System;
using System.Collections.Generic;
using System.Reflection;

namespace NServiceStub.WCF
{
    public class CapturedServiceMethodInvocation : IMessageInitializerParameterBinder
    {
        private readonly MethodInfo _serviceMethod;
        private readonly object[] _invocationArguments;

        public CapturedServiceMethodInvocation(MethodInfo serviceMethod, object[] invocationArguments)
        {
            _serviceMethod = serviceMethod;
            _invocationArguments = invocationArguments;
        }

        public object[] Bind<TMsg>(TMsg message, Delegate messageInitializer)
        {
            ParameterInfo[] destinationArguments = messageInitializer.Method.GetParameters();

            if (destinationArguments.Length == 0)
                return new object[] { };

            var argumentValues = new List<object> { message };

            if (destinationArguments[0].ParameterType != typeof(TMsg))
                throw new InvalidOperationException("The first parameter of the delegate must be the message to initialize");

            var mapper = new MapInputArgumentHeuristic(_serviceMethod, messageInitializer, 1);

            argumentValues.AddRange(mapper.Map(_invocationArguments));

            return argumentValues.ToArray();
        }
    }
}