using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Clay.Models.Domain
{
    public class AppIdentityUser : IdentityUser
    {
        public ICollection<UserLock> UserLockCollection { get; } = new List<UserLock>();
    }
}