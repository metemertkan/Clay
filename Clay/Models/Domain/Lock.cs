using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Clay.Models.Domain.Base;

namespace Clay.Models.Domain
{
    [Serializable]
    public class Lock : BaseModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Place { get; set; }
        public bool IsLocked { get; set; }

        public ICollection<UserLock> UserLockCollection { get; } = new List<UserLock>();
    }
}