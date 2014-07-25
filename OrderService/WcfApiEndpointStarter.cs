using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using Castle.Facilities.WcfIntegration;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NServiceBus;
using OrderService.Contracts;

namespace OrderService
{
    public class WcfApiEndpointStarter : IWantToRunWhenBusStartsAndStops
    {
        private ServiceHostBase _wcfEndPoint;

        private readonly IWindsorContainer _container;

        public WcfApiEndpointStarter(IWindsorContainer container)
        {
            _container = container;
            container.AddFacility<WcfFacility>();
            _container.Register(Component.For<IOrderService>().ImplementedBy<OrderService>());
        }

        public void Start()
        {
            _wcfEndPoint = CreateAndOpenWCFHost(typeof(IOrderService).AssemblyQualifiedName);
        }

        public void Stop()
        {
            _wcfEndPoint.Close();
        }

        private ServiceHostBase CreateAndOpenWCFHost(string constructorString)
        {
            ServiceHostBase serviceHost = new DefaultServiceHostFactory().CreateServiceHost(constructorString, new Uri[0]);
            ServiceEndpoint serviceEndpoint = serviceHost.Description.Endpoints.SingleOrDefault(x => x.Binding is BasicHttpBinding);

            if (!serviceHost.Description.Behaviors.Any(x => x is ServiceMetadataBehavior) && serviceEndpoint != null)
            {
                string uriString = serviceEndpoint.Address.Uri.ToString().Replace("localhost", Environment.MachineName);

                var metadataBehavior = new ServiceMetadataBehavior
                    {
                        HttpGetEnabled = true,
                        HttpGetUrl = new Uri(uriString)
                    };
                serviceHost.Description.Behaviors.Add(metadataBehavior);
            }
            serviceHost.Open();
            return serviceHost;
        }
    }
}