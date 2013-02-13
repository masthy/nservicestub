using System;
using System.Collections.Generic;
using System.Reflection;

namespace NServiceStub.Rest
{
    public class CapturedPostInvocation : IMessageInitializerParameterBinder
    {
        private readonly RequestWrapper _request;
        private readonly IRouteTemplate _routeOwningUrl;

        public CapturedPostInvocation(RequestWrapper request, IRouteTemplate routeOwningUrl)
        {
            _request = request;
            _routeOwningUrl = routeOwningUrl;
        }

        public object[] Bind<TMsg>(TMsg message, Delegate messageInitializer)
        {
            ParameterInfo[] destinationArguments = messageInitializer.Method.GetParameters();

            if (destinationArguments.Length == 0)
                return new object[] { };

            var argumentValues = new List<object> { message };

            if (destinationArguments[0].ParameterType != typeof(TMsg))
                throw new InvalidOperationException("The first parameter of the delegate must be the message to initialize");

            if (destinationArguments.Length == 2)
                argumentValues.Add(_request.NegotiateAndDeserializeMethodBody());
            else
            {
                throw new NotImplementedException("Not supported yet");
            }

            return argumentValues.ToArray();
        }
    }
}