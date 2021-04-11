using System.Collections.Generic;

namespace CoreNotify.Shared.Interfaces
{
    public interface IRecipient
    {
        string NotificationKey { get; }
        string Email { get; }
        string Subject { get; }
        IDictionary<string, object> Parameters { get; }
    }
}
