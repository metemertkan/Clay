using System;

namespace Clay.Services.Interfaces
{
    public interface IUserLockService
    {
        bool CanUserAccess(string userId, Guid lockId);
        void SaveUserLock(string userId, Guid lockId);
        void RemoveUserLock(string userId, Guid lockId);
    }
}