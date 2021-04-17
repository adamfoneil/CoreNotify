using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreNotify.Functions.Helpers
{
    public static class HttpRequestExtensions
    {
        public static async Task<(T item, string json)> DeserializeJsonAsync<T>(this HttpRequest request, ILogger log = null)
        {
            var json = await ReadBodyAsync(request);

            log?.LogDebug(json);

            return (JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            }), json);
        }

        public static async Task<T> DeserializeAsync<T>(this HttpRequest request)
        {
            return (await DeserializeJsonAsync<T>(request)).item;
        }

        public static async Task<string> ReadBodyAsync(this HttpRequest request) => 
            await new StreamReader(request.Body).ReadToEndAsync();

        /// <summary>
        /// output some reqest info in order to debug Refit
        /// </summary>
        public static void Debug(this HttpRequest request, ILogger log)
        {
            StringBuilder output = new StringBuilder();
            output.AppendLine($"Content Type: {request.ContentType}");
            output.AppendLine($"Length: {request.ContentLength}");

            var collections = new Dictionary<string, Func<HttpRequest, IEnumerable<KeyValuePair<string, StringValues>>>>()
            {
                ["Headers"] = (request) => request.Headers,
                ["Query"] = (request) => request.Query,
                ["Cookies"] = (request) => request.Cookies.Select(kp => new KeyValuePair<string, StringValues>(kp.Key, new StringValues(new string[] { kp.Value }))),
                ["Form"] = (request) => request.Form
            };

            foreach (var kp in collections)
            {
                var items = Enumerable.Empty<KeyValuePair<string, StringValues>>();
                
                try
                {
                    items = kp.Value.Invoke(request);
                }                    
                catch
                {
                    /* ignore, can't access Form when content type = application/json */
                }
                                
                if (items.Any())
                {
                    output.AppendLine($"{kp.Key}:");
                    foreach (var item in items) output.AppendLine($"- {item.Key} = {item.Value}");
                }                
            }

            log.LogDebug(output.ToString());
        }
    }
}
