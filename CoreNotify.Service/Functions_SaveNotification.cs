using CoreNotify.Database;
using Dapper.CX.Classes;
using Dapper.CX.SqlServer.Extensions.Int;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SetCronJob.ApiClient;
using SetCronJob.ApiClient.Models;
using System;
using System.Threading.Tasks;

namespace CoreNotify.Service
{
    public static partial class Functions
    {
        /// <summary>
        /// used to create (post) or update (put) a Notification object
        /// </summary>
        public static async Task<int> SaveNotificationAsync(
            SqlConnection connection, string accountName, string accountKey, 
            Notification notification, ISetCronJobClient cronJobClient, 
            string baseUrl, string callbackFunctionCode,
            ILogger logger)
        {
            var account = await AuthenticateAsync(connection, accountName, accountKey, logger);
            
            // notification must be in your account
            notification.AccountId = account.Id;            

            var cronJob = new CronJob()
            {
                Id = notification.CronJobId ?? 0,
                Expression = notification.Schedule,
                Name = $"{account.Name}.{notification.Name}",
                Group = account.Name,
                Status = (notification.IsActive) ? JobStatus.Active : JobStatus.Disabled,
                TimeZone = account.TimeZone,
                Method = "GET",
                Url = baseUrl + $"api/CronJobExecute?code={callbackFunctionCode}&accountId={account.Id}&name={notification.Name}"
            };

            // update cron job in backend service
            await UpdateCronJobAsync(notification, cronJobClient, cronJob);

            var user = new SystemUser(account.Name);

            // set the Id to 0 and merge so that you can't overwrite someone else's record
            notification.Id = 0;
            var notificationId = await connection.MergeAsync(notification, user: user);

            return notificationId;
        }

        private static async Task UpdateCronJobAsync(Notification notification, ISetCronJobClient cronJobClient, CronJob cronJob)
        {
            string errorContext = null;
            try
            {
                if (notification.IsCronJobEnabled && !notification.CronJobId.HasValue)
                {
                    // create new
                    errorContext = "creating";
                    var result = await cronJobClient.CreateJobAsync(cronJob);
                    notification.CronJobId = result.Id;                    
                }
                else if (notification.CronJobId.HasValue)
                {
                    // update or disable
                    errorContext = "updating";
                    await cronJobClient.UpdateJobAsync(cronJob);
                }
            }
            catch (Exception exc)
            {
                notification.CronJobMessage = exc.Message + $" while {errorContext}";
            }
        }
    }
}
