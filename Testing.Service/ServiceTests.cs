using CoreNotify.Database;
using CoreNotify.Service;
using CoreNotify.Shared;
using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlServer.LocalDb;
using System;
using Testing.Service.Helpers;

namespace Testing.Service
{
    [TestClass]
    public class ServiceTests
    {
        const string AccountName = "sample";
        const string AccountKey = "sample-account-key";
        const string NotificationName = "email1";

        /// <summary>
        /// make sure TestApp is running (ctrl+F5) first
        /// </summary>
        [TestMethod]
        public void ExecuteSend()
        {
            using (var cn = LocalDb.GetConnection("CoreNotify"))
            {
                CreateSampleObjects(cn);

                var logger = LoggerFactory.Create(config => config.AddConsole()).CreateLogger("CoreNotify");                

                var sendGridKey = ConfigHelper.GetValue["SendGrid:ApiKey"];
                Functions.ExecuteSend(cn, new SendRequest()
                {
                    AccountName = AccountName,
                    AccountKey = AccountKey,
                    EmailAddress = "adamosoftware@gmail.com",
                    NotificationName = NotificationName
                }, logger);
            }
        }

        [TestMethod]
        public void SaveNotification()
        {
            using (var cn = LocalDb.GetConnection("CoreNotify"))
            {
                CreateSampleObjects(cn);
                var logger = LoggerFactory.Create(config => config.AddConsole()).CreateLogger("CoreNotify");

                var notification = SampleNotification(0);

                var id = Functions.SaveNotificationAsync(cn, AccountName, AccountKey, notification, logger).Result;

                var testRow = cn.Get<Notification>(id);
                Assert.IsTrue(testRow.Name.Equals(NotificationName));
                Assert.IsTrue(testRow.AccountId != 0);
            }
        }

        private void CreateSampleObjects(SqlConnection cn)
        {
            var account = new Account()
            {
                Name = AccountName,
                PlanId = 1,
                AuthorizationKey = "sample-auth-key",
                RenewalDate = DateTime.Today.AddDays(30),
                SendGridApiKey = ConfigHelper.GetValue["SendGrid:ApiKey"],
                CreatedBy = "test",
                DateCreated = DateTime.Now
            };

            var id = cn.MergeAsync(account).Result;

            cn.MergeAsync(new AccountKey()
            {
                AccountId = id,
                Key = AccountKey,
                CreatedBy = "test",
                DateCreated = DateTime.Now
            }).Wait();
            Notification notification = SampleNotification(id);

            cn.MergeAsync(notification).Wait();
        }

        private static Notification SampleNotification(int accountId)
        {
            return new Notification()
            {
                AccountId = accountId,
                Name = NotificationName,
                SenderEmail = "adamosoftware@gmail.com",
                RecipientEndpoint = "https://localhost:44349/Email/Recipients",
                ContentEndpoint = "https://localhost:44349/Email/Content",
                CreatedBy = "test",
                DateCreated = DateTime.Now
            };
        }
    }
}
