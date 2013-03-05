using System;
using System.Collections.Generic;
using System.Reflection;

namespace NServiceStub.Rest
{
    public class CapturedRouteInvocation : IMessageInitializerParameterBinder
    {
        private readonly RequestWrapper _request;
        private readonly IRouteTemplate _routeOwningUrl;

        public CapturedRouteInvocation(RequestWrapper request, IRouteTemplate routeOwningUrl)
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
                int requestOffset = 1;

                if (destinationArguments[1].ParameterType == typeof(object) && destinationArguments[1].Name == "body")
                {
                    requestOffset++;
                    argumentValues.Add(_request.NegotiateAndDeserializeMethodBody());
                }

                var mapper = new MapRequestToDelegateHeuristic(_routeOwningUrl.Route, messageInitializer, requestOffset);

                argumentValues.AddRange(mapper.Map(_request.Request));
            }
            return argumentValues.ToArray();
        }
    }
}