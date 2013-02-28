using System.Reflection;

namespace NServiceStub.WCF
{
    public interface IInvocationMatcher
    {
        bool Matches(MethodInfo method, object[] arguments);

        MethodInfo InspectedMethod { get; }
    }
}