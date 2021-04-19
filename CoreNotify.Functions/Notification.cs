using CoreNotify.Database;
using CoreNotify.Functions.Classes;
using CoreNotify.Functions.Helpers;
using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CoreNotify.Functions
{
    public static class NotificationFunction
    {
        [FunctionName("Notification")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "delete", "put", Route = null)] HttpRequest request,
            ILogger log, ExecutionContext context) => await new NotificationHandler(context, log).ExecuteAsync(request);        
    }

    public class NotificationHandler : DbFunctionHandler<Account, Notification, int>
    {
        public NotificationHandler(ExecutionContext context, ILogger logger) : base(context, logger)
        {
        }

        protected override SqlConnection GetConnection() => Context.GetConnection();

        protected override async Task<Account> AuthenticateAsync(SqlConnection connection, HttpRequest request) =>
            await DbFunctionHelpers.AuthenticateAsync(connection, request, Logger);
        
        protected override async Task<int> CreateAsync(SqlConnection connection, Notification model) =>
            await connection.SaveAsync(model);

        protected override async Task UpdateAsync(SqlConnection connection, Notification model) =>
            await connection.SaveAsync(model);

        protected override async Task DeleteAsync(SqlConnection connection, int id) =>
            await connection.DeleteAsync<Notification>(id);

        protected override async Task<Notification> GetAsync(SqlConnection connection, int id) =>
            await connection.GetAsync<Notification>(id);

        protected override int GetKey(HttpRequest request) =>
            DbFunctionHelpers.GetKey(request);      
    }
}

