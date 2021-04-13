using CoreNotify.Functions.Queries;
using Dapper.QX;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlServer.LocalDb;

namespace Testing.Functions
{
    [TestClass]
    public class QueryTests
    {
        [TestMethod]
        public void ValidateAccountQuery() => QueryHelper.Test<ValidateAccount>(GetConnection);

        private SqlConnection GetConnection() => LocalDb.GetConnection("CoreNotify");
    }
}
