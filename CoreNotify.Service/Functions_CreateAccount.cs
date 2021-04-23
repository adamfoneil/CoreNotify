using CoreNotify.Database;
using Dapper.CX.Classes;
using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.Data.SqlClient;
using StringIdLibrary;
using System;
using System.Threading.Tasks;

namespace CoreNotify.Service
{
    public static partial class Functions
    {
        public static async Task<Account> CreateAccountAsync(Account account, SqlConnection connection)
        {
            if (account.PlanId == 0) account.PlanId = (int)Plans.Monthly;
            account.RenewalDate = DateTime.Today.AddDays(30);
            if (string.IsNullOrEmpty(account.ValidationKey)) account.ValidationKey = StringId.New(16, StringIdRanges.Upper | StringIdRanges.Lower | StringIdRanges.Numeric);

            var user = new SystemUser("system");

            var plan = await connection.GetAsync<Plan>(account.PlanId);
            account.Price = plan.Price;

            var id = await connection.InsertAsync(account, user: user);

            var accountKey = new AccountKey()
            {
                AccountId = id,
                Key = StringId.New(32, StringIdRanges.Lower | StringIdRanges.Numeric)
            };

            await connection.InsertAsync(accountKey, user: user);

            account.Key = accountKey.Key;

            return account;
        }
    }
}
