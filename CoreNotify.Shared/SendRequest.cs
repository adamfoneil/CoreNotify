using CoreNotify.Shared.Interfaces;
using System.Collections.Generic;

namespace CoreNotify.Shared
{
    public class SendRequest : ISendRequest
    {
        public string AccountName { get; set; }
        public string AccountKey { get; set; }
        public string NotificationName { get; set; }
        public string EmailAddress { get; set; }        
        public IDictionary<string, object> Parameters { get; set; }        
    }
}
