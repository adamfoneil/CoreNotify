﻿using AO.Models;
using CoreNotify.Database.Conventions;
using StringIdLibrary;
using System.ComponentModel.DataAnnotations;

namespace CoreNotify.Database
{
    [UniqueConstraint(nameof(Key))]
    public class Notification : BaseTable
    {
        [Key]
        [References(typeof(Account))]
        public int AccountId { get; set; }

        [Key]
        [MaxLength(50)]
        public string Name { get; set; }
        
        [MaxLength(255)]
        public string Subject { get; set; }

        [Required]
        [MaxLength(20)]
        public string Schedule { get; set; } = "0 23 * * *"; // every day at 11PM default
        
        /// <summary>
        /// where do we get the recipients for this email?
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string RecipientEndpoint { get; set; }

        /// <summary>
        /// where do we get the content for this email?
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string ContentEndpoint { get; set; }

        [Required]
        [MaxLength(20)]
        public string Key { get; set; } = StringId.New(16, StringIdRanges.Upper | StringIdRanges.Numeric);

        public bool IsActive { get; set; } = true;
    }
}