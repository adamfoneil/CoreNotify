using System.Collections.Generic;

namespace CoreNotify.Shared.Interfaces
{
    public interface IRecipient
    {
        string NotificationKey { get; }
        string EmailAddress { get; }        
        IDictionary<string, object> Parameters { get; }
    }
}
