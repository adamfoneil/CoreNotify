using AO.Models;
using AO.Models.Interfaces;
using CoreNotify.Database.Conventions;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading.Tasks;

namespace CoreNotify.Database
{
    [UniqueConstraint(nameof(Key))]
    public class Notification : BaseTable, IValidate
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

        [MaxLength(255)]        
        public string Subject { get; set; }

        [MaxLength(255)]
        public string SubjectEndpoint { get; set; }

        [MaxLength(20)]
        public string Schedule { get; set; } // cron job expression

        /// <summary>
        /// where do we get the recipients for this email?
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

        [Required]
        [MaxLength(50)]
        public string Key { get; set; }

        public bool IsActive { get; set; } = true;

        public ValidateResult Validate()
        {
            if (string.IsNullOrWhiteSpace(Subject) && string.IsNullOrWhiteSpace(SubjectEndpoint))
            {
                return ValidateResult.Failed("Must provide either Subject or SubjectEndpoint");
            }

            return ValidateResult.Ok();
        }

        public async Task<ValidateResult> ValidateAsync(IDbConnection connection, IDbTransaction txn = null) => await Task.FromResult(Validate());        
    }
}
