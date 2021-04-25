using CoreNotify.Database;
using Dapper.CX.Classes;
using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CoreNotify.Service
{
    public static partial class Functions
    {
        /// <summary>
        /// used to create (post) or update (put) a Notification object
        /// </summary>
        public static async Task SaveNotificationAsync(
            SqlConnection connection, string accountName, string accountKey, Notification notification, 
            ILogger logger)
        {
            var account = await AuthenticateAsync(connection, accountName, accountKey, logger);
            
            // notification must be in your account
            notification.AccountId = account.Id;

            var user = new SystemUser(account.Name);

            // I use merge instead of save so the Notification.Name takes precedence over the Id
            await connection.MergeAsync(notification, user: user);
        }
    }
}
