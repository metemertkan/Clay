using System;
using System.Collections.Generic;
using System.Linq;
using Clay.Models.Domain;

namespace Clay.Repositories.Interfaces
{
    public interface IUserLockRepository
    {
        IQueryable<UserLock> UserLocks { get; }
        void SaveUserLock(string userId, Guid lockId);
        void RemoveUserLock(string userId, Guid lockId);
        bool RelationExist(string userId, Guid lockId);
    }
}