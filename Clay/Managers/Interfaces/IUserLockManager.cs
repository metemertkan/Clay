using System;
using System.Threading.Tasks;

namespace Clay.Managers.Interfaces
{
    public interface IUserLockManager
    {
        Task<bool> Assign(string userId, Guid lockId);
        Task<bool> UnAssign(string userId, Guid lockId);
        Task<bool> CanAccess(string userId, Guid lockId);
    }
}