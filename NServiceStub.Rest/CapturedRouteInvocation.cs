using System;
using System.Collections.Generic;
using System.Reflection;

namespace NServiceStub.Rest
{
    public class CapturedRouteInvocation : IMessageInitializerParameterBinder
    {
        private readonly string _rawUrl;
        private readonly IRouteDefinition _routeOwningUrl;

        public CapturedRouteInvocation(string rawUrl, IRouteDefinition routeOwningUrl)
        {
            _rawUrl = rawUrl;
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
                var mapper = new MapQueryStringDelegateHeuristic(_routeOwningUrl.Route, messageInitializer, 1);

                argumentValues.AddRange(mapper.Map(_rawUrl));
            }
            return argumentValues.ToArray();
        }
    }
}