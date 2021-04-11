using System.Text.Json;

namespace CoreNotify.Functions.Helpers
{
    public static class JsonHelper
    {
        public static bool TryParse<T>(string json, out T result)
        {
            try
            {
                result = JsonSerializer.Deserialize<T>(json);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }
    }
}
