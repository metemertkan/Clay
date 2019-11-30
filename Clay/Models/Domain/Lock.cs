using System;
using System.Collections.Generic;

namespace Clay.Models.Domain
{
    public class Lock
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Place { get; set; }
        public bool IsLocked { get; set; }

        public ICollection<UserLock> UserLockCollection { get; } = new List<UserLock>();
    }
}