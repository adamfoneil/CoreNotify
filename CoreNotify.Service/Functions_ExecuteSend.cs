using CoreNotify.Database;
using CoreNotify.Service.Extensions;
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
        //static HttpClient client = new HttpClient();

        public static void ExecuteSend(
            SqlConnection connection, ISendRequest request, ILogger log, HttpClient client)
        {
            var account = AuthenticateAsync(connection, request.AccountName, request.AccountKey, log).Result;

            var notification = GetNotification(connection, account.Id, request.NotificationName, log);

            var email = GetEmail(account, notification, request, log, client);

            SendEmailAndLog(email, request, account, notification, log, client);
        }

        private static Notification GetNotification(
            SqlConnection connection, int accountId, string notificationName,
            ILogger log, [CallerMemberName] string callerName = null)
        {
            log.Trace(callerName, new { accountId, notificationName });

            var notification = connection.GetWhere<Notification>(new { name = notificationName, accountId });
            if (notification == null)
            {
                throw new Exception($"Notification key {notificationName} not found.");
            }

            return notification;
        }
      
        private static (string subject, string body) GetEmail(
            Account account, Notification notification, ISendRequest request, 
            ILogger log, HttpClient client, [CallerMemberName]string callerName = null)
        {
            log.Trace(callerName, new { account.Name });
            log.Trace(callerName, new { accountId = account.Id, notification.Name, notification.Id });
            log.Trace(callerName, request);
            
            try
            {
                var builder = new UriBuilder(notification.ContentEndpoint);
                builder.AddQueryParameters(request.Parameters);
                builder.AddQueryParameter(Notification.QueryStringKey, account.AuthorizationKey);

                string contentUrl = builder.Uri.AbsoluteUri;
                var response = client.GetAsync(contentUrl).Result;
                HttpStatusCode statusCode = response.StatusCode;
                response.EnsureSuccessStatusCode();                

                string body = response.Content.ReadAsStringAsync().Result;
                string subject = (!string.IsNullOrWhiteSpace(notification.Subject)) ?
                    notification.Subject :
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
            (string subject, string body) email, ISendRequest request,
            Account account, Notification notification, ILogger log, HttpClient httpClient,
            [CallerMemberName] string callerName = null)
        {
            log.Trace(callerName, account);
            log.Trace(callerName, notification);

            var sendGridClient= new SendGridClient(httpClient, new SendGridClientOptions {ApiKey = account.SendGridApiKey });
            //var sendGridClient = new SendGridClient(account.SendGridApiKey);

            var message = MailHelper.CreateSingleEmail(
                new EmailAddress(notification.SenderEmail),
                new EmailAddress(request.EmailAddress), 
                email.subject,
                null,
                email.body);

            sendGridClient.SendEmailAsync(message).Wait();

            log.Info(callerName, new { notification, request });
        }
    }
}
