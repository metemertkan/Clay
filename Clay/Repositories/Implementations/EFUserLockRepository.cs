using System;
using System.Linq;
using Clay.Data;
using Clay.Models.Domain;
using Clay.Repositories.Interfaces;

namespace Clay.Repositories.Implementations
{
    public class EFUserLockRepository : IUserLockRepository
    {
        private readonly WebDbContext _context;

        public EFUserLockRepository(WebDbContext context)
        {
            _context = context;
        }

        public IQueryable<UserLock> UserLocks => _context.Set<UserLock>();
        public void SaveUserLock(string userId, Guid lockId)
        {
            _context.Add(new UserLock
            {
                LockId = lockId,
                UserId = userId
            });
            _context.SaveChanges();
        }

        public void RemoveUserLock(string userId, Guid lockId)
        {
            var foundRelation = GetRelation(userId, lockId);
            if (foundRelation == null)
                return;
            _context.Remove(foundRelation);
            _context.SaveChanges();
        }

        public bool RelationExist(string userId, Guid lockId)
        {
            var foundRelation = GetRelation(userId, lockId);
            return foundRelation != null;
        }

        private UserLock GetRelation(string userId, Guid lockId)
        {
            return _context.Set<UserLock>().FirstOrDefault(ul => ul.UserId.Equals(userId) && ul.LockId.Equals(lockId));
        }
    }
}