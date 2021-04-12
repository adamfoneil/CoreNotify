using AO.Models;
using CoreNotify.Database.Conventions;
using System.ComponentModel.DataAnnotations;

namespace CoreNotify.Database
{
    public class AccountUser : BaseTable
    {
        [Key]
        [References(typeof(Account))]
        public int AccountId { get; set; }

        [Key]
        [References(typeof(UserProfile))]
        public int UserId { get; set; }
    }
}
