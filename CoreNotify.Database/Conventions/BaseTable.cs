using AO.Models;
using AO.Models.Enums;
using AO.Models.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace CoreNotify.Database.Conventions
{
    [Identity(nameof(Id))]
    public abstract class BaseTable : IAudit
    {
        public int Id { get; set; }

        [SaveAction(SaveAction.Insert)]
        public DateTime DateCreated { get; set; }

        [MaxLength(50)]
        [Required]
        [SaveAction(SaveAction.Insert)]
        public string CreatedBy { get; set; }

        [SaveAction(SaveAction.Update)]
        public DateTime? DateModified { get; set; }

        [MaxLength(50)]
        [SaveAction(SaveAction.Update)]
        public string ModifiedBy { get; set; }

        public void Stamp(SaveAction saveAction, IUserBase user)
        {
            switch (saveAction)
            {
                case SaveAction.Insert:
                    CreatedBy = user.Name;
                    DateCreated = user.LocalTime;
                    break;

                case SaveAction.Update:
                    ModifiedBy = user.Name;
                    DateModified = user.LocalTime;
                    break;
            }
        }
    }
}
