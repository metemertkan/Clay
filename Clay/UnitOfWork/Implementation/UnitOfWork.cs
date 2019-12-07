using System.Threading.Tasks;
using Clay.Data;
using Clay.Models.Domain;
using Clay.Repositories.Implementations;
using Clay.Repositories.Interfaces;
using Clay.UnitOfWork.Interfaces;

namespace Clay.UnitOfWork.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private ILockRepository _lockRepository;
        private IBaseRepository<UserLock> _userLockRepository;
        private IBaseRepository<Attempt> _attemptRepository;
        public WebDbContext DatabaseContext { get; }
        public UnitOfWork(WebDbContext context)
        {
            DatabaseContext = context;
        }

        public async Task<bool> Save()
        {
            try
            {
                int _save = await DatabaseContext.SaveChangesAsync();
                return await Task.FromResult(true);
            }
            catch (System.Exception e)
            {
                return await Task.FromResult(false);
            }
        }

        public ILockRepository LockRepository => _lockRepository ?? (_lockRepository = new EFLockRepository(DatabaseContext));

        public IBaseRepository<Attempt> AttemptRepository => _attemptRepository ?? (_attemptRepository = new EFAttemptRepository(DatabaseContext));

        public IBaseRepository<UserLock> UserLockRepository => _userLockRepository ?? (_userLockRepository = new EFUserLockRepository(DatabaseContext));

        public void Dispose()
        {
            DatabaseContext.Dispose();
        }
    }
}