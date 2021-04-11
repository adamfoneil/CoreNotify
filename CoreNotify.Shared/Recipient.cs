using CoreNotify.Shared.Interfaces;
using System.Collections.Generic;

namespace CoreNotify.Shared
{
    public class Recipient : IRecipient
    {
        public string NotificationKey { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public IDictionary<string, object> Parameters { get; set; }
    }
}
