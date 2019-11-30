using System.Linq;
using Clay.Models.Domain;

namespace Clay.Repositories.Interfaces
{
    public interface ILockRepository
    {
        IQueryable<Lock> Locks { get; }
        void SaveLock(Lock lockModel);
    }
}