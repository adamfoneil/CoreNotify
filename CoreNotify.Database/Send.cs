using AO.Models;
using CoreNotify.Database.Conventions;
using System.ComponentModel.DataAnnotations;

namespace CoreNotify.Database
{
    public class Send : LogTable
    {
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
