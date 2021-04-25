using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;

namespace CoreNotify.Service.Extensions
{
    public static class ILoggerExtensions
    {
        public static void Trace(this ILogger logger, string methodName, object data) =>
            LogInner(methodName, data, (message) => logger.LogTrace(message));        

        public static void Info(this ILogger logger, string methodName, object data) =>
            LogInner(methodName, data, (message) => logger.LogInformation(message));
        
        private static void LogInner(string methodName, object data, Action<string> action)
        {
            var options = new JsonSerializerOptions()
            {
                WriteIndented = true
            };

            action.Invoke($"{methodName}:\r\n\t{JsonSerializer.Serialize(data, options)}");
        }
    }
}
