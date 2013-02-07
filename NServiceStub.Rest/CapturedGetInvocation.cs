using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace NServiceStub.Rest
{
    public class CapturedGetInvocation : IMessageInitializerParameterBinder
    {
        private readonly HttpListenerRequest _request;
        private readonly IGetTemplate _routeOwningUrl;

        public CapturedGetInvocation(HttpListenerRequest request, IGetTemplate routeOwningUrl)
        {
            _request = request;
            _routeOwningUrl = routeOwningUrl;
        }

        public object[] Bind<TMsg>(TMsg message, Delegate messageInitializer)
        {
            ParameterInfo[] destinationArguments = messageInitializer.Method.GetParameters();

            if (destinationArguments.Length == 0)
                return new object[]{};
            
            var argumentValues = new List<object> {message};

            if (destinationArguments[0].ParameterType != typeof(TMsg))
                throw new InvalidOperationException("The first parameter of the delegate must be the message to initialize");

            if (destinationArguments.Length > 1)
            {
                var mapper = new MapRequestToDelegateHeuristic(_routeOwningUrl.Route, messageInitializer, 1);

                argumentValues.AddRange(mapper.Map(_request));
            }
            return argumentValues.ToArray();
        }
    }
}