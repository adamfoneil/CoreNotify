using AO.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreNotify.Models
{
    [Identity(nameof(Id))]
    public class Renewal
    {
        public int Id { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        [Required]
        public string PayerEmail { get; set; }

        [References(typeof(Account))]
        public int AccountId { get; set; }

        [References(typeof(Plan))]
        public int PlanId { get; set; }

        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        [MaxLength(255)]
        public string TransactionId { get; set; }
    }
}
