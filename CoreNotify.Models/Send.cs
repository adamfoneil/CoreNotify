using AO.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace CoreNotify.Models
{
    [Schema("log")]
    public class Send
    {        
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [References(typeof(Notification))]
        public int NotificationId { get; set; }

        [MaxLength(100)]
        [Required]
        public string Email { get; set; }

        [MaxLength(255)]
        [Required]
        public string Subject { get; set; }

        [Required]
        public string Content { get; set; }

        public bool Bounced { get; set; }
        
        public bool Unsubscribe { get; set; }
    }
}
