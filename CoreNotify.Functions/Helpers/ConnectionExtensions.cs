using CoreNotify.Database;
using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace CoreNotify.Functions.Helpers
{
    public static class ConnectionExtensions
    {
        public static void LogError(this SqlConnection connection, string message, object data)
        {
            connection.Insert(new Error()
            {
                Message = message,
                Data = JsonSerializer.Serialize(data)
            }, getIdentity: false);
        }
    }
}
