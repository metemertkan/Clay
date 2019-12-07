using System;
using System.Threading.Tasks;
using Clay.Managers.Interfaces;
using Clay.Models.Domain;
using Clay.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Clay.Managers.Implementations
{
    public class UserLockManager : IUserLockManager
    {
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public UserLockManager(UserManager<AppIdentityUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Assign(string userId, Guid lockId)
        {
            var (user, @lock) = await GetUserAndLock(userId, lockId);
            await _unitOfWork.UserLockRepository.Add(new UserLock
            {
                LockId = @lock.Id,
                UserId = user.Id
            });
            return await _unitOfWork.Save();
        }
        public async Task<bool> UnAssign(string userId, Guid lockId)
        {
            var (user, @lock) = await GetUserAndLock(userId, lockId);
            await _unitOfWork.UserLockRepository.Delete(l => l.UserId == user.Id && l.LockId == @lock.Id);
            return await _unitOfWork.Save();
        }
        public async Task<bool> CanAccess(string userId, Guid lockId)
        {
            var result = await _unitOfWork.UserLockRepository.FindBy(ul => ul.LockId == lockId && ul.UserId == userId);
            return result != null;
        }

        private async Task<Tuple<AppIdentityUser, Lock>> GetUserAndLock(string userId, Guid lockId)
        {
            var foundUser = _userManager.FindByIdAsync(userId).Result;
            if (foundUser == null)
                throw new Exception("User not found");

            var foundLock = await _unitOfWork.LockRepository.FindBy(l => l.Id == lockId);
            if (foundLock == null)
                throw new Exception("Lock not found");

            return new Tuple<AppIdentityUser, Lock>(foundUser, foundLock);
        }
    }
}