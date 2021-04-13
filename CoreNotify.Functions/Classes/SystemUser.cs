using AO.Models.Interfaces;
using System;

namespace CoreNotify.Functions.Classes
{
    public class SystemUser : IUserBase
    {
        public string Name => "system";
        public DateTime LocalTime => DateTime.UtcNow;
    }
}
