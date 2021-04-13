using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreNotify.Functions.Helpers
{
    public static class HttpRequestExtensions
    {
        public static async Task<(T item, string json)> DeserializeJsonAsync<T>(this HttpRequest request)
        {
            var json = await ReadBodyAsync(request);
            return (JsonSerializer.Deserialize<T>(json), json);
        }

        public static async Task<T> DeserializeAsync<T>(this HttpRequest request)
        {
            return (await DeserializeJsonAsync<T>(request)).item;
        }

        public static async Task<string> ReadBodyAsync(this HttpRequest request) => 
            await new StreamReader(request.Body).ReadToEndAsync();
    }
}
