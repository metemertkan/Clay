using Clay.Data;
using Clay.Models.Domain;

namespace Clay.Repositories.Implementations
{
    public class EFUserLockRepository : BaseRepository<UserLock>
    {
        public EFUserLockRepository(WebDbContext context) : base(context)
        { }
    }
}