using CoreNotify.Functions.Helpers;
using CoreNotify.Shared.Interfaces;
using CoreNotify.Database;
using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using Microsoft.Data.SqlClient;
using SendGrid;
using System.Net;

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
                if (JsonHelper.TryParse(message, out IRecipient recipient))
                {
                    using (var cn = context.GetConnection("DatabaseConnection"))
                    {                        
                        var request = ValidateRequest(cn, recipient);

                        var html = GetContent(cn, request, recipient);

                        SendEmailAndLog(cn, html, recipient, request.account.SendGridApiKey ?? context.GetSendGridKey());
                    }
                }
            }
            catch (Exception exc)
            {
                log.LogError(exc, exc.Message);
            }
        }

        private static string GetContent(SqlConnection cn, (Notification notification, Account account) request, IRecipient recipient)
        {
            string contentUrl = null;
            HttpStatusCode statusCode = 0;

            try
            {
                var builder = new UriBuilder(request.notification.ContentEndpoint);
                builder.AddQueryParameters(recipient.Parameters);
                builder.AddQueryParameter("key", request.account.ValidationKey);

                contentUrl = builder.Uri.AbsoluteUri;
                var response = client.GetAsync(contentUrl).Result;
                statusCode = response.StatusCode;
                response.EnsureSuccessStatusCode();

                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception exc)
            {
                cn.LogError(exc.Message, new
                {
                    statusCode,
                    notificationId = request.notification.Id,
                    contentUrl
                });

                throw;
            }
        }

        private static void SendEmailAndLog(SqlConnection cn, string html, IRecipient recipient, string sendGridKey)
        {
            var sendGridClient = new SendGridClient(sendGridKey);

        }

        private static (Notification notification, Account account) ValidateRequest(SqlConnection cn, IRecipient recipient)
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
