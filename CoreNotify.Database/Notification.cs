using AO.Models;
using CoreNotify.Database.Conventions;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace CoreNotify.Database
{
    public enum SubjectType
    {
        Static,
        Dynamic
    }

    [UniqueConstraint(nameof(Key))]
    public class Notification : BaseTable
    {
        [Key]
        [References(typeof(Account))]
        public int AccountId { get; set; }

        [Key]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(255)]
        [Required]
        public string SenderEmail { get; set;  }

        /// <summary>
        /// static subject line text (if null, then an "EmailSubject" header is expected from ContentEndpoint)
        /// </summary>
        [MaxLength(255)]        
        public string Subject { get; set; }

        /// <summary>
        /// cron job expression
        /// </summary>
        [MaxLength(20)]
        public string Schedule { get; set; }

        /// <summary>
        /// where do we get the recipients for this email? Called during cronjob trigger
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string RecipientEndpoint { get; set; }

        /// <summary>
        /// where do we get the content for this email?
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string ContentEndpoint { get; set; }

        /// <summary>
        /// unique identifier used to query this during execution
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Key { get; set; }

        public bool IsActive { get; set; } = true;

        public const string SubjectHeader = "EmailSubject";
    }
}
