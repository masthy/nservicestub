using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using NServiceStub.Rest.Configuration;
using Newtonsoft.Json;

namespace NServiceStub.Rest
{
    public class RestApi : IDisposable
    {
        private HttpListener _listener;
        private QueryStringParser _parser;
        private ServiceStub _service;

        private readonly IList<IRouteDefinition> _routeTable = new List<IRouteDefinition>();

        public RestApi(string baseUrl, QueryStringParser parser, ServiceStub service)
        {
            _parser = parser;
            _service = service;
            _listener = new HttpListener();
            _listener.Prefixes.Add(baseUrl);
            Start();
        }

        public IRouteDefinition<T> AddRouteGet<T>(string queryString)
        {
            Route route = _parser.Parse(queryString);

            var routeDefinition = new RouteDefinition<T>(route);
            _routeTable.Add(routeDefinition);
            return routeDefinition;
        }

        public RouteConfiguration<R> Configure<R>(IRouteDefinition<R> route)
        {
            return new RouteConfiguration<R>(route, _service);
        }

        private void Start()
        {
            _listener.Start();

            _listener.BeginGetContext(HandleRequest, null);
        }

        public void Dispose()
        {
            _listener.Stop();

            _listener = null;
            _parser = null;
            _service = null;
        }

        private static void WriteStringToResponse(HttpListenerResponse response, string serializeObject)
        {
            byte[] buffer = response.ContentEncoding.GetBytes(serializeObject);

            response.ContentLength64 = buffer.Length;
            using (Stream output = response.OutputStream)
            {
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
        }

        private void HandleRequest(IAsyncResult result)
        {
            if (!_listener.IsListening)
                return;

            HttpListenerContext context = _listener.EndGetContext(result);

            _listener.BeginGetContext(HandleRequest, null);

            WriteResponse(context);
        }

        private void WriteResponse(HttpListenerContext context)
        {
            context.Response.ContentEncoding = Encoding.UTF8;

            IRouteDefinition route = _routeTable.FirstOrDefault(definition => definition.Route.Matches(context.Request.RawUrl));

            if (route != null)
            {
                object returnValue;
                if (!route.TryInvocation(context.Request, out returnValue))
                {
                    returnValue = null;
                }

                string serializeObject = JsonConvert.SerializeObject(returnValue);
                WriteStringToResponse(context.Response, serializeObject);
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                WriteStringToResponse(context.Response, "Not Found");
            }
        }
    }
}