namespace NServiceStub
{
    public interface IFactory<T>
    {
        T Create();

        void Release(T instance);
    }
}