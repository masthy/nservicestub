using Moq;

namespace NServiceStub
{
    public class ProxyHelper
    {
        public static T CreateImpl<T>() where T : class
        {
            var proxy = new Mock<T>();
            proxy.SetupAllProperties();
            return proxy.Object;
        }
    }
}