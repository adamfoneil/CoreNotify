using CoreNotify.Database;
using CoreNotify.Service.Extensions;
using CoreNotify.Shared;
using CoreNotify.Shared.Interfaces;
using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SendGrid;
using System;
using System.Net;
using System.Net.Http;

namespace CoreNotify.Service
{
    public static partial class Functions
    {
        static HttpClient client = new HttpClient();

        public static void ExecuteSend(string sendGridKey, Recipient recipient, SqlConnection connection, ILogger log)
        {
            var request = ValidateRequest(connection, recipient);

            var html = GetContent(connection, request, recipient);

            SendEmailAndLog(connection, html, recipient, request.account.SendGridApiKey ?? sendGridKey);
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

        private static void SendEmailAndLog(SqlConnection cn, string html, IRecipient recipient, string sendGridKey)
        {
            var sendGridClient = new SendGridClient(sendGridKey);

        }

    }
}
