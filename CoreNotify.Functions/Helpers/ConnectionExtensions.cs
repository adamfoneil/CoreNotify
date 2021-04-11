using CoreNotify.Database;
using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.Data.SqlClient;
using System;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace CoreNotify.Functions.Helpers
{
    public static class ConnectionExtensions
    {
        public static void LogError(this SqlConnection connection, string message, object data, bool throwException = true, [CallerMemberName]string callerName = null)
        {
            connection.Insert(new Error()
            {
                MethodName = callerName,
                Message = message,
                Data = JsonSerializer.Serialize(data)
            }, getIdentity: false);

            if (throwException) throw new Exception(message);
        }
    }
}
