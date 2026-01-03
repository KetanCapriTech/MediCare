using MediCare.Helpers.Interface;
using System.Text.Json;

namespace MediCare.Helpers
{
    public class SessionHelper : ISessionHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session =>
            _httpContextAccessor.HttpContext?.Session
            ?? throw new InvalidOperationException("Session is not available.");

        public void SetString(string key, string value)
        {
            Session.SetString(key, value);
        }

        public string? GetString(string key)
        {
            return Session.GetString(key);
        }

        public void SetInt(string key, int value)
        {
            Session.SetInt32(key, value);
        }

        public int? GetInt(string key)
        {
            return Session.GetInt32(key);
        }

        public void SetObject<T>(string key, T value)
        {
            var json = JsonSerializer.Serialize(value);
            Session.SetString(key, json);
        }

        public T? GetObject<T>(string key)
        {
            var json = Session.GetString(key);
            return json == null ? default : JsonSerializer.Deserialize<T>(json);
        }

        public void Remove(string key)
        {
            Session.Remove(key);
        }

        public void Clear()
        {
            Session.Clear();
        }
    }
}
