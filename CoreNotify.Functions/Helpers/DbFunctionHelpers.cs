using CoreNotify.Database;
using CoreNotify.Functions.Queries;
using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoreNotify.Functions.Helpers
{
    public static class DbFunctionHelpers
    {
        public static async Task<Account> AuthenticateAsync(SqlConnection connection, HttpRequest request)
        {
            try
            {
                var result = await new ValidateAccount()
                {
                    Name = request.Headers["account"],
                    Key = request.Headers["key"]
                }.ExecuteSingleOrDefaultAsync(connection);

                if (result == null) throw new Exception("Account not found.");

                if (result.RenewalDate < DateTime.Now) throw new Exception($"Account expired as of {result.RenewalDate}");

                return result;
            }
            catch (Exception exc)
            {
                throw new Exception($"Error locating account: {exc.Message}");
            }
        }

        public static int GetKey(HttpRequest request)
        {
            try
            {
                var id = request.Query["id"].First();
                return int.Parse(id);
            }
            catch
            {
                return 0;
            }
        }
    }
}
