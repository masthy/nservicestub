using System;
using System.Collections.Generic;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Lifestyle.Scoped;

namespace NServiceStub.WCF
{
    public class OneProxyPrStubScopeAccessor : IScopeAccessor
    {
        Dictionary<ServiceStub, DefaultLifetimeScope> _createdScopes = new Dictionary<ServiceStub, DefaultLifetimeScope>();

        public void Dispose()
        {
            foreach (var defaultLifetimeScope in _createdScopes.Values)
            {
                defaultLifetimeScope.Dispose();
            }

            _createdScopes.Clear();
            _createdScopes = null;
        }

        public ILifetimeScope GetScope(CreationContext context)
        {
            foreach (var additionalArgument in context.AdditionalArguments.Values)
            {
                if (additionalArgument is ServiceStub)
                {
                    var serviceStub = additionalArgument as ServiceStub;

                    if (_createdScopes.ContainsKey(serviceStub))
                        return _createdScopes[serviceStub];
                    else
                    {
                        var scope = new DefaultLifetimeScope();
                        _createdScopes[serviceStub] = scope;
                        serviceStub.Disposing += RemoveScope;

                        return scope;
                    }
                }
            }

            throw new InvalidOperationException("This was not used as intented, I am expecting a servicestub as a provided argument");
        }

        private void RemoveScope(ServiceStub sender)
        {
            sender.Disposing -= RemoveScope;
            DefaultLifetimeScope scope = _createdScopes[sender];
            _createdScopes.Remove(sender);
            scope.Dispose();
        }
    }
}