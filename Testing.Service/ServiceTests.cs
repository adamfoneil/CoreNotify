using CoreNotify.Database;
using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlServer.LocalDb;
using System;

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
                CreateSampleAccountAndNotification(cn);


            }
        }

        private void CreateSampleAccountAndNotification(SqlConnection cn)
        {
            var account = new Account()
            {
                Name = "sample",
                PlanId = 1,
                ValidationKey = "hello",
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
                Key = "sample-key",
                CreatedBy = "test",
                DateCreated = DateTime.Now
            };

            cn.MergeAsync(notification).Wait();
        }
    }
}
