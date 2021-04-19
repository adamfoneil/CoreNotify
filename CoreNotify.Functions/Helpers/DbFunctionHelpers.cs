using CoreNotify.Database;
using CoreNotify.Functions.Queries;
using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CoreNotify.Functions.Helpers
{
    public static class DbFunctionHelpers
    {     
        public static int GetKey(HttpRequest request)
        {
            try
            {
                var id = request.Query["id"].First();
                return int.Parse(id);
            }
            catch
            {
                return 0;
            }
        }
    }
}
