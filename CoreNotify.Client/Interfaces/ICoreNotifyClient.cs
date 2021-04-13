using CoreNotify.Database;
using Refit;
using System.Threading.Tasks;

namespace CoreNotify.Interfaces
{        
    internal interface ICoreNotifyClient
    {
        [Post("/api/CreateAccount")]
        Task<Account> CreateAccountAsync([Body]Account account);

        [Post("/api/UpdateAccount")]
        Task UpdateAccountAsync([Body] Account account, [Header("account")]string accountName, [Header("key")]string accountKey);

        [Post("/api/Notification")]
        Task<Notification> SaveNotificationAsync([Body] Notification notification, [Header("account")]string accountName, [Header("key")]string accountKey);
    }
}
