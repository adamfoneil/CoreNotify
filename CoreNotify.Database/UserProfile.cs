using AO.Models;
using AO.Models.Models;

namespace CoreNotify.Database
{
    public class UserProfile : UserProfileBase
    {
        [References(typeof(Account))]
        public int? AccountId { get; set; }
    }
}
