namespace MediCare.Helpers.Interface
{
    public interface ISessionHelper
    {
        void SetString(string key, string value);
        string? GetString(string key);

        void SetInt(string key, int value);
        int? GetInt(string key);

        void SetObject<T>(string key, T value);
        T? GetObject<T>(string key);

        void Remove(string key);
        void Clear();
    }
}
