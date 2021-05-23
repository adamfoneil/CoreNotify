using AO.Models.Models;
using System.Text.Json;

namespace CoreNotify.Database
{
    public class QueuedJob : BackgroundJobInfo<int>
    {
        protected override T DeserializeJson<T>(string json) => JsonSerializer.Deserialize<T>(json);

        public const string ZipBuilderQueue = "zip-builder";
    }
}
