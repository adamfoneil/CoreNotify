using CoreNotify.Functions.Helpers;
using CoreNotify.Shared;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace CoreNotify.Functions
{
    public class ExecuteSend
    {
        private readonly HttpClient httpClient;

        public ExecuteSend(IHttpClientFactory httpClientFactory)
        {
            this.httpClient = httpClientFactory.CreateClient();
        }

        public const string QueueName = "corenotify-send";

        [FunctionName("ExecuteSend")]
        public void Run(
            [QueueTrigger(QueueName, Connection = "StorageAccount")] string message,
            ILogger log, ExecutionContext context)
        {
            try
            {
                if (JsonHelper.TryParse(message, out SendRequest request))
                {
                    using (var cn = context.GetConnection())
                    {
                        Service.Functions.ExecuteSend(cn, request, log, httpClient);
                    }
                }
            }
            catch (Exception exc)
            {
                log.LogError(exc, exc.Message);
            }
        }
    }
}
