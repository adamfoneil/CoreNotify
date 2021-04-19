using CoreNotify.Database;
using CoreNotify.Service.Queries;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CoreNotify.Service
{
    public static partial class Functions
    {
        public static async Task<Account> AuthenticateAsync(SqlConnection connection, string accountName, string accountKey, ILogger log)
        {
            try
            {
                var qry = new ValidateAccount()
                {
                    Name = accountName,
                    Key = accountKey
                };

                var result = await qry.ExecuteSingleOrDefaultAsync(connection);

                if (result == null) throw new Exception($"Account {qry.Name}:{qry.Key} not found.");

                if (result.RenewalDate < DateTime.Now) throw new Exception($"Account expired as of {result.RenewalDate}");

                return result;
            }
            catch (Exception exc)
            {
                log?.LogError(exc, $"Error locating account: {exc.Message}");
                throw new Exception($"Error locating account: {exc.Message}");
            }
        }

    }
}
