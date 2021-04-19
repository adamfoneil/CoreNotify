using CoreNotify.Database;
using CoreNotify.Functions.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
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
            request.Debug(log);

            try
            {
                var account = await request.DeserializeAsync<Account>();

                using (var cn = context.GetConnection())
                {
                    account = await Service.Functions.CreateAccountAsync(account, cn);
                }

                return new OkObjectResult(account);                
            }
            catch (Exception exc)
            {
                log.LogError(exc, exc.Message);
                return new BadRequestObjectResult(exc.Message);
            }
        }
    }
}

