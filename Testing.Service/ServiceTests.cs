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

                var sendGridKey = ConfigHelper.Config["SendGrid:ApiKey"];
                Functions.ExecuteSend(sendGridKey, new Recipient()
                {
                    EmailAddress = "adamosoftware@gmail.com",
                    NotificationKey = "sample-notification"                    
                }, cn, logger);
            }
        }

        private void CreateSampleObjects(SqlConnection cn)
        {
            var account = new Account()
            {
                Name = "sample",
                PlanId = 1,
                AuthorizationKey = "sample-auth-key",
                CreatedBy = "test",
                DateCreated = DateTime.Now
            };

            var id = cn.MergeAsync(account).Result;

            var notification = new Notification()
            {
                AccountId = id,
                Name = "sample",
                SenderEmail = "adamosoftware@gmail.com",
                RecipientEndpoint = "https://localhost:44349/Email/Recipients",
                ContentEndpoint = "https://localhost:44349/Email/Content",
                Key = "sample-notification",
                CreatedBy = "test",
                DateCreated = DateTime.Now
            };

            cn.MergeAsync(notification).Wait();
        }
    }
}
