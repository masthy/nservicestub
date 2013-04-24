using System;
using Castle.DynamicProxy.Generators;

namespace NServiceStub.WCF
{
    public class ProxyNamer : INamingScope
    {
        private readonly NamingScope _wrappedScope;
        private readonly System.Type _serviceContract;

        public ProxyNamer(NamingScope wrappedScope, Type serviceContract)
        {
            _wrappedScope = wrappedScope;
            _serviceContract = serviceContract;
        }

        private ProxyNamer(INamingScope parentScope, NamingScope wrappedScope, Type serviceContract) : this(wrappedScope, serviceContract)
        {
            ParentScope = parentScope;
        }

        public string GetUniqueName(string suggestedName)
        {
            if (suggestedName.StartsWith("Castle.Proxies.") && suggestedName.EndsWith("Proxy"))
                return String.Format("{0}Stub", _serviceContract.FullName);
            else
                return _wrappedScope.GetUniqueName(suggestedName);
        }

        public INamingScope SafeSubScope()
        {
            return new ProxyNamer(this, _wrappedScope, _serviceContract);
        }

        public INamingScope ParentScope { get; private set; }
    }
}