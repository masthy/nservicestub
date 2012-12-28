using System.Reflection;

namespace NServiceStub.WCF
{
    public interface IInvocationMatcher
    {
        bool Matches(object[] arguments);

        MethodInfo InspectedMethod { get; }
    }
}