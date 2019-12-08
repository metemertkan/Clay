using System;
using System.ComponentModel.DataAnnotations;

namespace Clay.Models.InputModels.Admin
{
    public class UserLockModel
    {
        [Required]
        public string UserId { get; set; }
        [Required]
        public Guid LockId { get; set; }
    }
}