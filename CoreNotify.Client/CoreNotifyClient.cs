using CoreNotify.Interfaces;
using Refit;

namespace CoreNotify
{
    public class CoreNotifyClient
    {        
        private readonly ICoreNotifyClient _api;
        private readonly string _account;
        private readonly string _key;

        public CoreNotifyClient(string host, string account, string key)
        {
            Host = host;

            _api = RestService.For<ICoreNotifyClient>(Host);
            _account = account;
            _key = key;
        }

        public string Host { get; }

    }
}
