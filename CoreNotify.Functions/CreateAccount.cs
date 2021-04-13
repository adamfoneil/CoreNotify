using CoreNotify.Database;
using CoreNotify.Functions.Classes;
using CoreNotify.Functions.Helpers;
using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using StringIdLibrary;
using System;
using System.Threading.Tasks;

namespace CoreNotify.Functions
{
    public static class CreateAccount
    {
        [FunctionName("CreateAccount")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest request,
            ILogger log, ExecutionContext context)
        {
            try
            {
                var account = await request.DeserializeAsync<Account>();
                if (account.PlanId == 0) account.PlanId = (int)Plans.Monthly;
                account.RenewalDate = DateTime.Today.AddDays(30);
                if (string.IsNullOrEmpty(account.ValidationKey)) account.ValidationKey = StringId.New(16, StringIdRanges.Upper | StringIdRanges.Lower | StringIdRanges.Numeric);

                var user = new SystemUser();

                using (var cn = context.GetConnection())
                {
                    var plan = await cn.GetAsync<Plan>(account.PlanId);
                    account.Price = plan.Price;

                    var id = await cn.InsertAsync(account, user: user);

                    var accountKey = new AccountKey()
                    {
                        AccountId = id,
                        Key = StringId.New(32, StringIdRanges.Upper | StringIdRanges.Lower | StringIdRanges.Numeric | StringIdRanges.Special)
                    };

                    await cn.InsertAsync(accountKey, user: user);

                    account.Key = accountKey.Key;

                    return new OkObjectResult(account);
                }                
            }
            catch (Exception exc)
            {
                log.LogError(exc, exc.Message);
                return new BadRequestObjectResult(exc.Message);
            }
        }
    }
}

