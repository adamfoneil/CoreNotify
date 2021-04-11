using CoreNotify.Functions.Helpers;
using CoreNotify.Shared.Interfaces;
using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace CoreNotify.Functions
{
    public static class Send
    {
        [FunctionName("Send")]
        public static void Run(
            [QueueTrigger("corenotify-recipients", Connection = "StorageAccount")]string message, 
            ILogger log, ExecutionContext context)
        {
            if (JsonHelper.TryParse(message, out ICoreNotifyRecipient recipient))
            {
                using (var cn = context.GetConnection("DatabaseConnection"))
                {
                    var notification = cn.Get<Notification>(recipient.NotificationId);
                }
            }

            log.LogInformation($"C# Queue trigger function processed: {message}");
        }
    }
}
