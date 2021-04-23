using Azure.Storage.Queues;
using CoreNotify.Functions.Helpers;
using CoreNotify.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CoreNotify.Functions
{
    /// <summary>
    /// called by CoreNotifyClient to initiate a send
    /// </summary>
    public static class QueueSend
    {
        [FunctionName("QueueSend")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest request,
            ILogger log, ExecutionContext context)
        {
            try
            {
                var connectionStr = context.GetConfig()["StorageAccount"];
                var queueClient = new QueueClient(connectionStr, ExecuteSend.QueueName);

                var body = await request.DeserializeJsonAsync<Recipient>();
                var receipt = await queueClient.SendAsync(body.json);

                return new OkObjectResult(receipt);
            }
            catch (Exception exc)
            {
                log.LogError(exc, exc.Message);
                return new BadRequestObjectResult(exc.Message);
            }
        }
    }
}

