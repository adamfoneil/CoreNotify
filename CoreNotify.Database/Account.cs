using AO.Models;
using CoreNotify.Database.Conventions;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreNotify.Database
{
    public class Account : BaseTable
    {
        [Key]
        [MaxLength(50)]
        public string Name { get; set; }

        [References(typeof(Plan))]
        public int PlanId { get; set; }

        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        public DateTime RenewalDate { get; set; } = DateTime.Today.AddDays(30);

        /// <summary>
        /// used to validate recipient and rendering requests in your application(s)
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string ValidationKey { get; set; }

        /// <summary>
        /// BYO SendGrid key if you want
        /// </summary>
        [MaxLength(255)]
        public string SendGridApiKey { get; set; }

        /// <summary>
        /// used during initial account creation only, not populated on gets because there can be many
        /// </summary>
        [NotMapped]
        public string Key { get; set; }
    }
}
