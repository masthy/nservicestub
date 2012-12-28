using System;

namespace NServiceStub
{
    /// <summary>
    /// Produces an array of parameters which
    /// matches the signature of the message initializer delegate
    /// </summary>
    public interface IMessageInitializerParameterBinder
    {
        object[] Bind<TMsg>(TMsg message, Delegate messageInitializer);
    }
}