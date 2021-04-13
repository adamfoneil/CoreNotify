using CoreNotify;
using CoreNotify.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            var client = new CoreNotifyClient("http://localhost:7071/");
            var result = client.CreateAccountAsync(new Account()
            {
                Name = "hello"
            }).Result;

            Assert.IsTrue(result != null);
        }
    }
}
