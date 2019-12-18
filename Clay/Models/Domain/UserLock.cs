using System;
using Clay.Models.Domain.Base;

namespace Clay.Models.Domain
{
    [Serializable]
    public class UserLock : BaseModel
    {
        public string UserId { get; set; }
        public AppIdentityUser User { get; set; }

        public Guid LockId { get; set; }
        public Lock Lock { get; set; }
    }
}