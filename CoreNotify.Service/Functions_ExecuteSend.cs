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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;

namespace CoreNotify.Service
{
    public static partial class Functions
    {
        static HttpClient client = new HttpClient();

        public static void ExecuteSend(
            string sendGridKey, IRecipient recipient, SqlConnection connection, 
            ILogger log)
        {          
            var request = ValidateRequest(connection, recipient, log);           

            var email = GetEmail(request, recipient, log);

            SendEmailAndLog(email, recipient, request, sendGridKey, log);
        }

        private static (Notification notification, Account account) ValidateRequest(
            SqlConnection cn, IRecipient recipient,
            ILogger log, [CallerMemberName]string callerName = null)
        {
            log.Trace(callerName, recipient);

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

            var blocked = cn.GetWhere<Unsubscribe>(new { email = recipient.EmailAddress, notificationId = recipient.NotificationKey });
            if (blocked != null) throw new Exception($"Recipient {recipient.EmailAddress} has unsubscribed from {notification.Name}, send skipped");

            return (notification, account);
        }

        private static (string subject, string body) GetEmail(
            (Notification notification, Account account) request, IRecipient recipient, 
            ILogger log, [CallerMemberName]string callerName = null)
        {
            log.Trace(callerName, request.account);
            log.Trace(callerName, request.notification);
            log.Trace(callerName, recipient);
            
            try
            {
                var builder = new UriBuilder(request.notification.ContentEndpoint);
                builder.AddQueryParameters(recipient.Parameters);
                builder.AddQueryParameter("key", request.account.ValidationKey);

                string contentUrl = builder.Uri.AbsoluteUri;
                var response = client.GetAsync(contentUrl).Result;
                HttpStatusCode statusCode = response.StatusCode;
                response.EnsureSuccessStatusCode();                

                string body = response.Content.ReadAsStringAsync().Result;
                string subject = (!string.IsNullOrWhiteSpace(request.notification.Subject)) ?
                    request.notification.Subject :
                    GetSubjectFromResponse(response);

                return (subject, body);                
            }
            catch (Exception exc)
            {
                log.LogError(exc, $"GetContent error: {exc.Message}");
                throw;
            }

            string GetSubjectFromResponse(HttpResponseMessage response)
            {
                var collections = new Func<HttpResponseMessage, HttpResponseHeaders>[]
                {
                    (response) => response.Headers,
                    (response) => response.TrailingHeaders
                };

                foreach (var collection in collections)
                {
                    if (collection.Invoke(response).TryGetValues(Notification.SubjectHeader, out IEnumerable<string> values))
                    {
                        return values.First();
                    }
                }

                throw new Exception($"Expected header {Notification.SubjectHeader} not found.");
            }
        }

        private static void SendEmailAndLog(
            (string subject, string body) email, IRecipient recipient, 
            (Notification notification, Account account) request, string sendGridKey, 
            ILogger log, [CallerMemberName]string callerName = null)
        {
            log.Trace(callerName, recipient);

            var sendGridClient = new SendGridClient(request.account.SendGridApiKey ?? sendGridKey);

            var message = MailHelper.CreateSingleEmail(
                new EmailAddress(request.notification.SenderEmail),
                new EmailAddress(recipient.EmailAddress), 
                email.subject,
                null,
                email.body);

            sendGridClient.SendEmailAsync(message).Wait();
        }
    }
}
