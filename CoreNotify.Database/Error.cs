using CoreNotify.Database.Conventions;
using System.ComponentModel.DataAnnotations;

namespace CoreNotify.Database
{
    /// <summary>
    /// condition that cause a process to stop
    /// </summary>
    public class Error : LogTable
    {        
        [Required]
        public string Message { get; set; }

        [Required]
        public string Data { get; set; }
    }
}
