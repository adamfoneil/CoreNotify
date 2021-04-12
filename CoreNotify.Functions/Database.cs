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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CoreNotify.Functions
{
    public static class Database
    {
        [FunctionName("Database")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", "put", "delete", Route = null)] HttpRequest request,
            ILogger log, ExecutionContext context)
        {
            try
            {
                using (var cn = context.GetConnection("DatabaseConnection"))
                {
                    var account = await GetAccountAsync(cn, request);
                    var tableName = GetModelType(request);
                    var body = await new StreamReader(request.Body).ReadToEndAsync();
                    
                    if (HttpMethods.IsGet(request.Method))
                    {


                    }
                    else if (HttpMethods.IsPut(request.Method))
                    {

                    }
                    else if (HttpMethods.IsDelete(request.Method))
                    {

                    }
                    else if (HttpMethods.IsPost(request.Method))
                    {

                    }
                }                
            }
            catch (Exception exc)
            {
                return new BadRequestObjectResult(exc.Message);
            }            
        }

        private static async Task<Account> GetAccountAsync(SqlConnection connection, HttpRequest request)
        {
            try
            {
                var result = await connection.GetWhereAsync<Account>(new
                {
                    name = request.Query["account"],
                    validationKey = request.Query["key"]
                });

                if (result == null) throw new Exception("Account not found.");

                if (result.RenewalDate < DateTime.Now) throw new Exception($"Account expired as of {result.RenewalDate}");

                return result;
            }
            catch (Exception exc)
            {
                throw new Exception($"Error locating account: {exc.Message}");
            }
        }

        private static SqlAdapter GetModelType(HttpRequest request)
        {
            try
            {
                var tableName = request.Query["table"].First();

                var tables = new Dictionary<string, SqlAdapter>()
                {
                    [nameof(Notification)] = new SqlAdapter(typeof(Notification)),
                    [nameof(Account)] = new SqlAdapter(typeof(Account))
                };

                return tables[tableName];
            }
            catch (Exception exc)
            {
                throw new Exception($"Error getting table name: {exc.Message}");
            }
        }
    }
}

