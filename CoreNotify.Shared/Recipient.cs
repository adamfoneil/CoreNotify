using CoreNotify.Shared.Interfaces;
using System.Collections.Generic;

namespace CoreNotify.Shared
{
    public class Recipient : IRecipient
    {
        public string NotificationKey { get; set; }
        public string EmailAddress { get; set; }        
        public IDictionary<string, object> Parameters { get; set; }
    }
}
