using AO.Models;
using CoreNotify.Database.Conventions;
using System;
using System.ComponentModel.DataAnnotations;

namespace CoreNotify.Database
{
    public class AccountKey : BaseTable
    {
        [Key]
        [MaxLength(255)]
        public string Key { get; set; }

        [References(typeof(Account))]
        public int AccountId { get; set; }

        public DateTime? LastUsed { get; set; }

        public bool IsEnabled { get; set; } = true;
    }
}
