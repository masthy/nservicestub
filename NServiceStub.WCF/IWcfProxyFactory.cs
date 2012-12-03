﻿using System;

namespace NServiceStub.WCF
{
    public interface IWcfProxyFactory : IExtensionBoundToStubLifecycle, IDisposable
    {
        WcfProxy<T> Create<T>(string httpEndpoint) where T : class;

        void Release<T>(WcfProxy<T> createdProxy) where T : class;
    }
}