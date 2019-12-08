using System;
using System.ComponentModel.DataAnnotations;

namespace Clay.Models.InputModels.Admin
{
    public class LockActionModel
    {
        [Required]
        public Guid LockId { get; set; }
    }
}