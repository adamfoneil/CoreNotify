using CoreNotify;
using CoreNotify.Database;
using Dapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlServer.LocalDb;
using StringIdLibrary;

namespace Testing
{
    /// <summary>
    /// you need to be running the Function project locally first
    /// </summary>
    [TestClass]
    public class ApiClientTests
    {
        [TestMethod]
        public void CreateAccount()
        {
            using (var cn = LocalDb.GetConnection("CoreNotify"))
            {
                cn.Execute(
                    @"DELETE [ak] FROM [dbo].[AccountKey] [ak] INNER JOIN [dbo].[Account] [a] ON [ak].[AccountId]=[a].[Id] WHERE [a].[Name]='hello';
                    DELETE [dbo].[Account] WHERE [Name]='hello'");
            }

            var client = new CoreNotifyClient("http://localhost:7071/");
            var result = client.CreateAccountAsync(new Account()
            {
                Name = "hello",
                SendGridApiKey = "whatever"
            }).Result;

            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public void UpdateAccount()
        {
            var client = new CoreNotifyClient("http://localhost:7071/");
            var result = client.CreateAccountAsync(new Account()
            {
                Name = "sample-" + StringId.New(4, StringIdRanges.Numeric | StringIdRanges.Upper),
                SendGridApiKey = "whatever"
            }).Result;

            client = new CoreNotifyClient("http://localhost:7071/", result.Name, result.Key);
            client.UpdateAccountAsync(new Account()
            {
                Name = "new-name-" + StringId.New(4, StringIdRanges.Numeric | StringIdRanges.Upper),
                PlanId = 2
            }).Wait();
        }
    }
}
