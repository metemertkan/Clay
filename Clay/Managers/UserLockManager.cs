using System;
using Clay.Models.Domain;
using Microsoft.AspNetCore.Identity;

namespace Clay.Managers
{
    public class UserLockManager
    {
        private readonly UserManager<AppIdentityUser> _userManager;
        public UserLockManager(UserManager<AppIdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public void Assign(Guid userId, Guid lockId)
        {

        }
        public void UnAssign(Guid userId, Guid lockId)
        {

        }
        public bool CanAccess(Guid userId, Guid lockId)
        {
            return false;
        }
    }
}