using System;

namespace Clay.Managers.Interfaces
{
    public interface IUserLockManager
    {
        void Assign(string userId, Guid lockId);
        void UnAssign(string userId, Guid lockId);
        bool CanAccess(string userId, Guid lockId);
    }
}