using CoreNotify.Functions.Helpers;
using CoreNotify.Shared;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;

namespace CoreNotify.Functions
{
    public static class ExecuteSend
    {
        public const string QueueName = "corenotify-send";

        [FunctionName("ExecuteSend")]
        public static void Run(
            [QueueTrigger(QueueName, Connection = "StorageAccount")] string message,
            ILogger log, ExecutionContext context)
        {
            try
            {
                if (JsonHelper.TryParse(message, out SendRequest recipient))
                {
                    using (var cn = context.GetConnection())
                    {
                        Service.Functions.ExecuteSend(cn, recipient, log);
                    }
                }
            }
            catch (Exception exc)
            {
                log.LogError(exc, exc.Message);
            }
        }
    }
}
