using System;
using System.Collections.Specialized;

namespace NServiceStub.Rest
{
    public interface IRoute
    {
        object GetRouteParameterValue(string name, string rawUrl, Type expectedType);
        object GetHeaderParameterValue(string parameterName, NameValueCollection headers, Type expectedType);
        bool Matches(string rawUrl);
        object GetQueryParameterValue(string name, string rawUrl, Type expectedType);
    }
}