using System.Text.Json;

namespace GlobalCalendar.MVC.SessionManagement
{
    public static class SessionExtensions
    {
        public static void SetObject<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static T GetObject<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
#pragma warning disable CS8603 // Possible null reference return.
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
#pragma warning restore CS8603 // Possible null reference return.
        }
    }
}
