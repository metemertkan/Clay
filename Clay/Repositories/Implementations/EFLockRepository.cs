using System;
using System.Linq;
using System.Threading.Tasks;
using Clay.Data;
using Clay.Models.Domain;
using Clay.Repositories.Interfaces;

namespace Clay.Repositories.Implementations
{
    public class EFLockRepository : BaseRepository<Lock>,ILockRepository
    {
        private WebDbContext _context;
        public EFLockRepository(WebDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> Lock(Lock @lock)
        {
            return await ExecuteAction(@lock.Id, true);
        }

        public async Task<bool> Unlock(Lock @lock)
        {
            return await ExecuteAction(@lock.Id, false);
        }

        public Task<bool> ExecuteAction(Guid id, bool @lock)
        {
            var foundLock = _context.Locks.FirstOrDefault(l => l.Id.Equals(id));

            if (foundLock == null)
            {
                //log lock not found
                return Task.FromResult(false);
            }

            if (foundLock.IsLocked == @lock)
            {
                //log lock is already in that state
                return Task.FromResult(false);
            }

            foundLock.IsLocked = @lock;
            return Task.FromResult(true);
        }
    }
}