using CoreNotify.Database;
using CoreNotify.Functions.Classes;
using CoreNotify.Functions.Helpers;
using Dapper.CX.Classes;
using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CoreNotify.Functions
{
    public static class UpdateAccount
    {
        [FunctionName("UpdateAccount")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest request,
            ILogger log, ExecutionContext context)
        {
            try
            {
                using (var cn = context.GetConnection())
                {
                    var account = await DbFunctionHelpers.AuthenticateAsync(cn, request);

                    var updateAccount = await request.DeserializeAsync<Account>();
                    var ct = new ChangeTracker<Account>(updateAccount);

                    // you can update only the account that you verified
                    updateAccount.Id = account.Id;
                    
                    var plan = await cn.GetAsync<Plan>(updateAccount.PlanId);
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

                    var user = new Classes.SystemUser();
                    await cn.SaveAsync(updateAccount, ct, user: user);

                    return new OkResult();
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

