using CoreNotify.Database;
using CoreNotify.Interfaces;
using Refit;
using System.Threading.Tasks;

namespace CoreNotify
{
    public class CoreNotifyClient
    {        
        private readonly ICoreNotifyClient _api;
        private readonly string _account;
        private readonly string _key;

        public CoreNotifyClient(string host)
        {
            Host = host;
            _api = RestService.For<ICoreNotifyClient>(Host);
        }

        public CoreNotifyClient(string host, string account, string key) : this(host)
        {
            _account = account;
            _key = key;
        }

        public string Host { get; }

        public async Task<Account> CreateAccountAsync(Account account) => await _api.CreateAccountAsync(account);
    }
}
