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
        private readonly UrlParser _postParser;
        private ServiceStub _service;

        private readonly IList<IRouteTemplate> _routeTable = new List<IRouteTemplate>();

        public RestApi(string baseUrl, QueryStringParser parser, UrlParser postParser, ServiceStub service)
        {
            _parser = parser;
            _postParser = postParser;
            _service = service;
            _listener = new HttpListener();
            _listener.Prefixes.Add(baseUrl);
            Start();
        }

        public IPostTemplate AddPost(string url)
        {
            Post route = _postParser.Parse(url);

            var template = new PostTemplate(route);
            _routeTable.Add(template);
            return template;
        }

        public IGetTemplate<T> AddGet<T>(string queryString)
        {
            Get route = _parser.Parse(queryString);

            var template = new GetTemplate<T>(route);
            _routeTable.Add(template);
            return template;
        }

        public GetTemplateConfiguration<R> Configure<R>(IGetTemplate<R> route)
        {
            return new GetTemplateConfiguration<R>(route, _service);
        }

        public PostTemplateConfiguration Configure(IPostTemplate route)
        {
            return new PostTemplateConfiguration(route, _service);
        }

        private void Start()
        {
            _listener.Start();

            _listener.BeginGetContext(HandleRequest, null);
        }

        public void Dispose()
        {
            _listener.Stop();
            _listener.Close();

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
            var listener = _listener;

            if (listener == null || !listener.IsListening)
                return;

            HttpListenerContext context;
            try
            {
                context = listener.EndGetContext(result);
            }
            catch (Exception)
            {
                return;
            }

            listener.BeginGetContext(HandleRequest, null);

            WriteResponse(context);
        }

        private void WriteResponse(HttpListenerContext context)
        {
            context.Response.ContentEncoding = Encoding.UTF8;

            var requestWrapper = new RequestWrapper(context.Request);

            IRouteTemplate route = _routeTable.FirstOrDefault(definition => definition.Matches(requestWrapper));

            if (route != null)
            {
                object returnValue;
                if (!route.TryInvocation(requestWrapper, out returnValue))
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