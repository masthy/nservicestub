using System;
using System.Net;

namespace NServiceStub.Rest
{
    public class CapturedPostInvocation : IMessageInitializerParameterBinder
    {
        private readonly HttpListenerRequest _request;
        private readonly IPostTemplate _routeOwningUrl;

        public CapturedPostInvocation(HttpListenerRequest request, IPostTemplate routeOwningUrl)
        {
            _request = request;
            _routeOwningUrl = routeOwningUrl;
        }

        public object[] Bind<TMsg>(TMsg message, Delegate messageInitializer)
        {
            throw new NotImplementedException();
        }
    }
}