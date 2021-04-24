using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CoreNotify.Service.Extensions
{
    public static class ILoggerExtensions
    {
        public static void Trace(this ILogger logger, string methodName, object data)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            logger.LogTrace($"{methodName}:\r\n\t{JsonSerializer.Serialize(data, options)}");
        }
    }
}
