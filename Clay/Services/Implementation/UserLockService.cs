using System;
using Clay.Repositories.Interfaces;
using Clay.Services.Interfaces;

namespace Clay.Services.Implementation
{
    public class UserLockService : IUserLockService
    {
        private readonly IUserLockRepository _userLockRepository;

        public UserLockService(IUserLockRepository userLockRepository)
        {
            _userLockRepository = userLockRepository;
        }

        public bool CanUserAccess(string userId, Guid lockId)
        {
            return _userLockRepository.RelationExist(userId, lockId);
        }

        public void SaveUserLock(string userId, Guid lockId)
        {
            _userLockRepository.SaveUserLock(userId, lockId);
        }

        public void RemoveUserLock(string userId, Guid lockId)
        {
            _userLockRepository.RemoveUserLock(userId, lockId);
        }
    }
}