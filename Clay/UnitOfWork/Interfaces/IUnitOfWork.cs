using System;
using System.Threading.Tasks;
using Clay.Data;
using Clay.Models.Domain;
using Clay.Repositories.Interfaces;

namespace Clay.UnitOfWork.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ILockRepository LockRepository { get; }
        IBaseRepository<Attempt> AttemptRepository { get; }
        IBaseRepository<UserLock> UserLockRepository { get; }
        WebDbContext DatabaseContext { get; }
        Task<bool> Save();
    }
}