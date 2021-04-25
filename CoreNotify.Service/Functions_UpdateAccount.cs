using CoreNotify.Database;
using Dapper.CX.Classes;
using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CoreNotify.Service
{
    public static partial class Functions
    {
        public static async Task UpdateAccount(
            SqlConnection connection, string accountName, string accountKey, Account updateAccount, ILogger logger)
        {
            var account = await AuthenticateAsync(connection, accountName, accountKey, logger);

            var ct = new ChangeTracker<Account>(updateAccount);

            // you can update only the account that you verified
            updateAccount.Id = account.Id;

            var plan = await connection.GetAsync<Plan>(updateAccount.PlanId);
            if (updateAccount.PlanId != account.PlanId && updateAccount.PlanId != 0)
            {
                // if you change plans, you get that plan price
                updateAccount.Price = plan.Price;
            }
            else
            {
                // otherwise price stays unchanged (set here to prevent user from changing)
                updateAccount.Price = account.Price;
            }

            // can't change the renewal date except by renewing (set here to prevent user from changing)
            updateAccount.RenewalDate = account.RenewalDate;

            var user = new SystemUser(account.Name);
            await connection.SaveAsync(updateAccount, ct, user: user);
        }
    }
}
