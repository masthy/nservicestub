using System.Collections.Generic;
using System.Reflection;

namespace OrderService.Contracts
{
    public static class CommandRegistry
    {
        public static IEnumerable<System.Type> GetCommandTypes(ICustomAttributeProvider provider)
        {
            yield return typeof(DeleteOrder);
            yield return typeof(DoSomethingWithOrder);
        }
            
    }
}