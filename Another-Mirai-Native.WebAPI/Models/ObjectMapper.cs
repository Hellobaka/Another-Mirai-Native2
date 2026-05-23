using System.Text.Json;

namespace Another_Mirai_Native.WebAPI.Models
{
    public static class ObjectMapper
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static T MapTo<T>(this object obj)
        {
#pragma warning disable IL2026
            var json = JsonSerializer.Serialize(obj, _options);
            return JsonSerializer.Deserialize<T>(json, _options)!;
#pragma warning restore IL2026
        }
    }
}
