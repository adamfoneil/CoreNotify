using CoreNotify.Database;
using CoreNotify.Service.Extensions;
using CoreNotify.Shared;
using CoreNotify.Shared.Interfaces;
using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace CoreNotify.Service
{
    public static partial class Functions
    {
        static HttpClient client = new HttpClient();

        public static void ExecuteSend(string sendGridKey, Recipient recipient, SqlConnection connection, ILogger log)
        {          
            var request = ValidateRequest(connection, recipient, log);

            var subject = (!string.IsNullOrWhiteSpace(request.notification.SubjectEndpoint)) ?
                GetSubject(request.notification.SubjectEndpoint, recipient, log) :
                request.notification.Subject;

            var html = GetContent(request, recipient, log);

            SendEmailAndLog(html, recipient, request, sendGridKey, log);
        }

        private static string GetSubject(string subjectEndpoint, Recipient recipient, ILogger log)
        {
            log.LogTrace($"GetSubject: {subjectEndpoint}");
            log.LogTrace($"GetSubject: {JsonSerializer.Serialize(recipient)}");

            throw new NotImplementedException();
        }

        private static string GetContent((Notification notification, Account account) request, IRecipient recipient, ILogger log)
        {            
            log.LogTrace($"GetContent account: {JsonSerializer.Serialize(request.account)}");
            log.LogTrace($"GetContent notfication: {JsonSerializer.Serialize(request.notification)}");
            log.LogTrace($"GetContent recipient: {JsonSerializer.Serialize(recipient)}");
            
            try
            {
                var builder = new UriBuilder(request.notification.ContentEndpoint);
                builder.AddQueryParameters(recipient.Parameters);
                builder.AddQueryParameter("key", request.account.ValidationKey);

                string contentUrl = builder.Uri.AbsoluteUri;
                var response = client.GetAsync(contentUrl).Result;
                HttpStatusCode statusCode = response.StatusCode;
                response.EnsureSuccessStatusCode();

                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception exc)
            {
                log.LogError(exc, $"GetContent error: {exc.Message}");
                throw;
            }
        }

        private static (Notification notification, Account account) ValidateRequest(SqlConnection cn, IRecipient recipient, ILogger log)
        {
            log.LogTrace($"ValidateRequest: {recipient}");

            var notification = cn.GetWhere<Notification>(new { key = recipient.NotificationKey });
            if (notification == null)
            {
                throw new Exception($"Notification key {recipient.NotificationKey} not found.");                
            }

            var account = cn.Get<Account>(notification.AccountId);
            if (account.RenewalDate < DateTime.Now)
            {
                throw new Exception($"Account {account.Name} expired on {account.RenewalDate}");                
            }

            var blocked = cn.GetWhere<Unsubscribe>(new { email = recipient.Email, notificationId = recipient.NotificationKey });
            if (blocked != null) throw new Exception($"Recipient {recipient.Email} has unsubscribed from {notification.Name}, send skipped");

            return (notification, account);
        }

        private static void SendEmailAndLog(string html, IRecipient recipient, (Notification notification, Account account) request, string sendGridKey, ILogger log)
        {
            log.LogTrace("SendEmailAndLog ");

            var sendGridClient = new SendGridClient(request.account.SendGridApiKey ?? sendGridKey);

            var message = MailHelper.CreateSingleEmail(
                new EmailAddress(request.notification.SenderEmail),
                new EmailAddress(recipient.Email), 
                request.notification.Subject,
                null,
                html);

            sendGridClient.SendEmailAsync(message).Wait();
        }

    }
}
