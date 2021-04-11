using CoreNotify.Functions.Helpers;
using CoreNotify.Shared.Interfaces;
using CoreNotify.Database;
using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;

namespace CoreNotify.Functions
{
    public static class Send
    {
        [FunctionName("Send")]
        public static void Run(
            [QueueTrigger("corenotify-send", Connection = "StorageAccount")]string message, 
            ILogger log, ExecutionContext context)
        {
            if (JsonHelper.TryParse(message, out ICoreNotifyRecipient recipient))
            {
                using (var cn = context.GetConnection("DatabaseConnection"))
                {
                    var notification = cn.Get<Notification>(recipient.NotificationId);
                    if (notification == null)
                    {
                        cn.LogError($"Notification {recipient.NotificationId} not found.", new { notificationId = recipient.NotificationId });
                    }

                    var account = cn.Get<Account>(notification.AccountId);
                    if (account.RenewalDate < DateTime.Now) cn.LogError($"Account {account.Name} expired", new
                    {
                        notificationId = recipient.NotificationId,
                        accountId = notification.AccountId
                    });
                }
            }

            log.LogInformation($"C# Queue trigger function processed: {message}");
        }
    }
}
