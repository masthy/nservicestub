using System;

namespace NServiceStub.Rest
{
    public interface IRestStubFactory : IExtensionBoundToStubLifecycle, IDisposable
    {
        RestApi Create(string baseUrl, ServiceStub service);

        void Release(RestApi createdComponent);
    }
}