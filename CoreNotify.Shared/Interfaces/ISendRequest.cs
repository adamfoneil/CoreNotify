using System.Collections.Generic;

namespace CoreNotify.Shared.Interfaces
{
    public interface ISendRequest
    {        
        string AccountName { get; }
        string AccountKey { get; }
        string NotificationName { get; }
        string EmailAddress { get; }        
        IDictionary<string, object> Parameters { get; }
    }
}
