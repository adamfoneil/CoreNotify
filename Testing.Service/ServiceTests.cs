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
                ValidationKey = "hello"
            };

            var id = cn.MergeAsync(account).Result;

            var notification = new Notification()
            {
                AccountId = id,
                Name = "sample",
                SenderEmail = "adamosoftware@gmail.com",
                RecipientEndpoint = "http://localhost"
            };
        }
    }
}
