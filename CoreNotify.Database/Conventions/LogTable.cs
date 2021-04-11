using AO.Models;
using System;

namespace CoreNotify.Database.Conventions
{
    [Schema("log")]
    public abstract class LogTable
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
