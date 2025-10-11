using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace YourProject.Helpers
{
    public static class SessionExtensions
    {
        public static void SetObjectAsJson(this ISession session, string key, object value)
            => session.SetString(key, JsonSerializer.Serialize(value));

        public static T? GetObjectFromJson<T>(this ISession session, string key)
            => session.GetString(key) == null ? default : JsonSerializer.Deserialize<T>(session.GetString(key));
    }
}
