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
        Task UpdateAccountAsync([Body] Account account, [Header("AccountName")]string accountName, [Header("AccountKey")]string accountKey);

        [Post("/api/Notification")]
        Task<Notification> SaveNotificationAsync([Body] Notification notification, [Header("AccountName")] string accountName, [Header("AccountKey")] string accountKey);
    }
}
