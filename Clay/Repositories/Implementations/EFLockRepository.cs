using System;
using System.Linq;
using Clay.Data;
using Clay.Models.Domain;
using Clay.Repositories.Interfaces;

namespace Clay.Repositories.Implementations
{
    public class EFLockRepository : ILockRepository
    {
        private WebDbContext _context;

        public EFLockRepository(WebDbContext context)
        {
            _context = context;
        }

        public IQueryable<Lock> Locks => _context.Locks;
        public void SaveLock(Lock lockModel)
        {
            if (lockModel.Id.Equals(Guid.Empty))
            {
                _context.Locks.Add(lockModel);
            }
            else
            {
                var dbEnty = _context.Locks.FirstOrDefault(l => l.Id.Equals(lockModel.Id));
                if (dbEnty!=null)
                {
                    dbEnty.Name = lockModel.Name;
                    dbEnty.IsLocked = lockModel.IsLocked;
                    dbEnty.Place = lockModel.Place;
                }
            }

            _context.SaveChanges();
        }
    }
}