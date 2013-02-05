using System;
using System.IO;
using System.Net;

namespace NServiceStub.Rest
{
    public class RequestWrapper
    {
        private object _deserializedBody;

        public RequestWrapper(HttpListenerRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            Request = request;
        }

        public object NegotiateAndDeserializeMethodBody()
        {
            if (_deserializedBody == null)
                _deserializedBody = Newtonsoft.Json.JsonConvert.DeserializeObject(ReadBodyAsString(Request));

            return _deserializedBody;

        }

        private static string ReadBodyAsString(HttpListenerRequest request)
        {
            string bodyAsString;

            using (var str = request.InputStream)
            {
                using (var reader = new StreamReader(str, request.ContentEncoding))
                {
                    bodyAsString = reader.ReadToEnd();
                }
            }
            return bodyAsString;
        }

        public HttpListenerRequest Request { get; private set; }
    }
}