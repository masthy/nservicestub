namespace NServiceStub
{
    public interface IIExtensionBoundToStubLifecycleFactory
    {
        IExtensionBoundToStubLifecycle[] Resolve();

        void Release(IExtensionBoundToStubLifecycle extension);
    }
}