using CoreNotify;
using CoreNotify.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Testing
{
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
