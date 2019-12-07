using System;

namespace Clay.Models.InputModels.Admin
{
    public class UserLockModel
    {
        public string UserId { get; set; }
        public Guid LockId { get; set; }
    }
}