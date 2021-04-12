using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreNotify.Functions.Classes
{
    public abstract class SqlFunctionHandler<TAccount, TModel, TKey>
    {
        protected abstract SqlConnection GetConnection();

        protected abstract Task<TAccount> AuthenticateAsync(SqlConnection connection, HttpRequest request);

        protected TAccount Account { get; private set; }

        protected abstract TKey GetKey(HttpRequest request);

        protected abstract Task<TKey> CreateAsync(SqlConnection connection, TModel model);

        protected abstract Task UpdateAsync(SqlConnection connection, TModel model);

        protected abstract Task DeleteAsync(SqlConnection connection, TKey id);

        protected abstract Task<TModel> GetAsync(SqlConnection connection, TKey id);

        public async Task<IActionResult> ExecuteAsync(HttpRequest request, ILogger logger)
        {
            try
            {
                using (var cn = GetConnection())
                {
                    Account = await AuthenticateAsync(cn, request);                    

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
                            var model = await GetModelBodyAsync(request);
                            var id = await CreateAsync(cn, model);
                            return new OkObjectResult(id);
                        }
                        else if (HttpMethods.IsPut(request.Method))
                        {
                            var model = await GetModelBodyAsync(request);
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
                return new BadRequestObjectResult(exc.Message);
            }
        }

        private static async Task<TModel> GetModelBodyAsync(HttpRequest request)
        {
            var json = await new StreamReader(request.Body).ReadToEndAsync();
            return JsonSerializer.Deserialize<TModel>(json);
        }
    }
}
