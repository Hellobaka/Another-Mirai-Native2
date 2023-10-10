using Newtonsoft.Json;

namespace Another_Mirai_Native
{
    public static class Helper
    {
        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.None);
        }
    }
}