using CoreNotify.Functions.Helpers;
using CoreNotify.Shared.Interfaces;
using CoreNotify.Database;
using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using Microsoft.Data.SqlClient;

namespace CoreNotify.Functions
{
    public static class Send
    {
        static HttpClient client = new HttpClient();

        [FunctionName("Send")]
        public static void Run(
            [QueueTrigger("corenotify-send", Connection = "StorageAccount")]string message, 
            ILogger log, ExecutionContext context)
        {            
            try
            {
                if (JsonHelper.TryParse(message, out ICoreNotifyRecipient recipient))
                {
                    using (var cn = context.GetConnection("DatabaseConnection"))
                    {                        
                        var request = ValidateRequest(cn, recipient);

                        var html = BuildHtml(request.notification.ContentEndpoint, recipient);

                        SendEmailAndLog(cn, html, recipient, request.account.SendGridApiKey ?? context.GetSendGridKey());
                    }
                }
            }
            catch (Exception exc)
            {
                log.LogError(exc, exc.Message);
            }
        }

        private static void SendEmailAndLog(SqlConnection cn, string html, ICoreNotifyRecipient recipient, string sendGridKey)
        {
            

        }

        private static string BuildHtml(string contentEndpoint, ICoreNotifyRecipient recipient)
        {
            throw new NotImplementedException();
        }

        private static (Notification notification, Account account) ValidateRequest(SqlConnection cn, ICoreNotifyRecipient recipient)
        {
            var notification = cn.GetWhere<Notification>(new { key = recipient.NotificationKey });
            if (notification == null)
            {
                cn.LogError($"Notification key {recipient.NotificationKey} not found.", new { key = recipient.NotificationKey });
            }

            var account = cn.Get<Account>(notification.AccountId);
            if (account.RenewalDate < DateTime.Now) cn.LogError($"Account {account.Name} expired on {account.RenewalDate}", new
            {
                key = recipient.NotificationKey,
                accountId = notification.AccountId,
                email = recipient.Email
            });

            var blocked = cn.GetWhere<Unsubscribe>(new { email = recipient.Email, notificationId = recipient.NotificationKey });
            if (blocked != null) cn.LogError($"Recipient {recipient.Email} has unsubscribed from {notification.Name}, send skipped", new
            {
                email = recipient.Email,
                notificationId = notification.Id
            });

            return (notification, account);
        }
    }
}
