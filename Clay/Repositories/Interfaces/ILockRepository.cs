using System.Threading.Tasks;
using Clay.Models.Domain;

namespace Clay.Repositories.Interfaces
{
    public interface ILockRepository : IBaseRepository<Lock>
    {
        Task<bool> Lock(Lock @lock);
        Task<bool> Unlock(Lock @lock);
    }
}