using AO.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoreNotify.Database
{
    [Identity(nameof(Id))]
    [UniqueConstraint(nameof(ItemNumber))]
    public class Plan
    {
        public int Id { get; set; }

        [MaxLength(50)]
        [Key]
        public string Name { get; set; }

        /// <summary>
        /// T-SQL DATEADD interval to calculated next renewal date
        /// </summary>
        [MaxLength(50)]
        [Required]
        public string DateAddInterval { get; set; }

        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        [MaxLength(50)]
        [Required]
        public string ItemNumber { get; set; }
    }
}
