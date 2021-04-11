using System.Collections.Generic;

namespace CoreNotify.Shared.Interfaces
{
    public interface ICoreNotifyRecipient
    {
        int NotificationId { get; }
        string Email { get; }
        string Subject { get; }
        IDictionary<string, object> Parameters { get; }
    }
}
