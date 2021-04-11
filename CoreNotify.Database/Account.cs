using AO.Models;
using CoreNotify.Models.Conventions;
using System;
using System.ComponentModel.DataAnnotations;

namespace CoreNotify.Models
{
    public class Account : BaseTable
    {
        [Key]
        [MaxLength(50)]        
        public string Name { get; set; }

        [References(typeof(Plan))]
        public int PlanId { get; set; }

        public DateTime RenewalDate { get; set; } = DateTime.Today.AddDays(30);

        [MaxLength(50)]
        public string QueryStringKey { get; set; }
    }
}
