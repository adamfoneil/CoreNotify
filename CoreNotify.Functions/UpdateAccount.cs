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
                var creds = request.GetCredentials();
                var updateAccount = await request.DeserializeAsync<Account>();

                using (var cn = context.GetConnection())
                {                    
                    await Service.Functions.UpdateAccount(cn, creds.name, creds.key, updateAccount, log);
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

