namespace _Project.Application.Persistence
{
    public interface IStorageProvider
    {
        void Save<T>(string key, T data) where T : class;
        T Load<T>(string key) where T : class, new();
        bool Exists(string key);
        void Delete(string key);
    }
}
