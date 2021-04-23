using CoreNotify.Functions.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CoreNotify.Functions.Classes
{
    public abstract class DbFunctionHandler<TAccount, TModel, TKey>
    {
        public DbFunctionHandler(ExecutionContext context, ILogger logger)
        {
            Context = context;
            Logger = logger;
        }

        protected ExecutionContext Context { get; }
        protected ILogger Logger { get; }

        protected abstract SqlConnection GetConnection();

        protected abstract Task<TAccount> AuthenticateAsync(SqlConnection connection, HttpRequest request);

        protected TAccount Account { get; private set; }

        protected abstract TKey GetKey(HttpRequest request);

        protected abstract Task<TKey> CreateAsync(SqlConnection connection, TModel model);

        protected abstract Task UpdateAsync(SqlConnection connection, TModel model);

        protected abstract Task DeleteAsync(SqlConnection connection, TKey id);

        protected abstract Task<TModel> GetAsync(SqlConnection connection, TKey id);

        public async Task<IActionResult> ExecuteAsync(HttpRequest request)
        {
            try
            {
                using (var cn = GetConnection())
                {
                    Account = await AuthenticateAsync(cn, request);
                    if (Account == null) throw new Exception("Request not validated");

                    if (HttpMethods.IsGet(request.Method))
                    {
                        var id = GetKey(request);
                        var result = await GetAsync(cn, id);
                        return new OkObjectResult(result);
                    }
                    else
                    {
                        if (HttpMethods.IsPost(request.Method))
                        {
                            var model = await request.DeserializeAsync<TModel>();
                            var id = await CreateAsync(cn, model);
                            return new OkObjectResult(id);
                        }
                        else if (HttpMethods.IsPut(request.Method))
                        {
                            var model = await request.DeserializeAsync<TModel>();
                            await UpdateAsync(cn, model);
                            return new OkResult();
                        }
                        else if (HttpMethods.IsDelete(request.Method))
                        {
                            var id = GetKey(request);
                            await DeleteAsync(cn, id);
                            return new OkResult();
                        }

                        return new BadRequestObjectResult($"Unsupported method: {request.Method}");
                    }
                }
            }
            catch (Exception exc)
            {
                Logger.LogError(exc, exc.Message);
                return new BadRequestObjectResult(exc.Message);
            }
        }
    }
}
