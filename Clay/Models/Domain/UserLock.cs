using System;

namespace Clay.Models.Domain
{
    public class UserLock
    {
        public string UserId { get; set; }
        public AppIdentityUser User { get; set; }

        public Guid LockId { get; set; }
        public Lock Lock { get; set; }
    }
}