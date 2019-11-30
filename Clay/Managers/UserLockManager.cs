using System;
using Clay.Models.Domain;
using Clay.Repositories.Interfaces;
using Clay.Services;
using Clay.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Clay.Managers
{
    public class UserLockManager
    {
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly ILockService _lockService;
        private readonly IUserLockRepository _userLockRepository;
        public UserLockManager(UserManager<AppIdentityUser> userManager, ILockService lockService, IUserLockRepository userLockRepository)
        {
            _userManager = userManager;
            _lockService = lockService;
            _userLockRepository = userLockRepository;
        }

        public void Assign(string userId, Guid lockId)
        {
            var (user, @lock) = GetUserAndLock(userId, lockId);
            _userLockRepository.SaveUserLock(user.Id, @lock.Id);
        }
        public void UnAssign(string userId, Guid lockId)
        {
            var (user, @lock) = GetUserAndLock(userId, lockId);
            _userLockRepository.RemoveUserLock(user.Id, @lock.Id);
        }
        public bool CanAccess(string userId, Guid lockId)
        {
            return _userLockRepository.RelationExist(userId, lockId);
        }

        private Tuple<AppIdentityUser, Lock> GetUserAndLock(string userId, Guid lockId)
        {
            var foundUser = _userManager.FindByIdAsync(userId).Result;
            if (foundUser == null)
                throw new Exception("User not found");

            var foundLock = _lockService.GetById(lockId);
            if (foundLock == null)
                throw new Exception("Lock not found");

            return new Tuple<AppIdentityUser, Lock>(foundUser, foundLock);
        }
    }
}