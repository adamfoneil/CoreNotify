using AO.Models;
using CoreNotify.Database.Conventions;
using System.ComponentModel.DataAnnotations;

namespace CoreNotify.Database
{
    public class Unsubscribe : BaseTable
    {
        [Key]
        [References(typeof(Notification))]
        public int NotificationId { get; set; }

        [Key]
        [MaxLength(100)]
        public string Email { get; set; }
    }
}
