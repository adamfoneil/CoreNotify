using AO.Models;
using CoreNotify.Database.Conventions;
using System;
using System.ComponentModel.DataAnnotations;
using StringIdLibrary;

namespace CoreNotify.Database
{
    public class Account : BaseTable
    {
        [Key]
        [MaxLength(50)]        
        public string Name { get; set; }

        [References(typeof(Plan))]
        public int PlanId { get; set; }

        public DateTime RenewalDate { get; set; } = DateTime.Today.AddDays(30);

        /// <summary>
        /// used to validate recipient and rendering requests in your application(s)
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string QueryStringKey { get; set; } = StringId.New(16, StringIdRanges.Lower | StringIdRanges.Numeric);

        /// <summary>
        /// BYO SendGrid key
        /// </summary>
        [MaxLength(255)]
        [Required]
        public string SendGridApiKey { get; set; }
    }
}
